using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping.Areas.Admin.Repository;
using Shopping.Models;
using Shopping.Repository;
using Shopping.Services.VnPay;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shopping.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        public CheckoutController(DataContext context, IEmailSender emailSender, IVnPayService vnPayService)
        {
            _dataContext = context;
            _emailSender = emailSender;
            _vnPayService = vnPayService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Checkout(string paymentMedthod, string paymentId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = orderCode;
                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                decimal shippingPrice = 0;
                var coupon_code = Request.Cookies["CouponTitle"];

                if (shippingPriceCookie != null)
                {
                    var shippingPriceJson = shippingPriceCookie;
                    shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
                }
                orderItem.ShippingCost = shippingPrice;
                orderItem.UserName = userEmail;
                if (paymentMedthod != "VnPay")
                {
                    orderItem.PaymentMethod = "Tiền mặt";
                }
                else
                {
                    orderItem.PaymentMethod = "VnPay " + paymentId;
                }
                orderItem.Status = 1;
                orderItem.CreatedDate = DateTime.Now;
                orderItem.CouponCode = coupon_code;

                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();

                List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartItem)
                {
                    var orderDetails = new OrderDetails();
                    orderDetails.UserName = userEmail;
                    orderDetails.OrderCode = orderCode;
                    orderDetails.ProductId = cart.ProductID;
                    orderDetails.Price = cart.Price;
                    orderDetails.Quantity = cart.Quantity;

                    var product = await _dataContext.Products.Where(p => p.Id == cart.ProductID).FirstAsync();
                    product.Quantity -= cart.Quantity;
                    product.Sold += cart.Quantity;
                    _dataContext.Update(product);

                    _dataContext.Add(orderDetails);
                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                //Send mail order when success
                var receiver = userEmail;
                var subject = "Đặt hàng thành công";
                var message = "Đơn hàng đã được đặt. Vui lòng đợi duyệt đơn!";

                await _emailSender.SendEmailAsync(receiver, subject, message);
                TempData["success"] = "Đơn hàng đã được tạo. Vui lòng đợi duyệt đơn!";
                return RedirectToAction("History", "Account");
            }
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode == "00")
            {
                var newVnpayInsert = new VnpayModel
                {
                    OrderId = response.OrderId,
                    PaymentMethod = response.PaymentMethod,
                    OrderDescription = response.OrderDescription,
                    TransactionId = response.TransactionId,
                    PaymentId = response.PaymentId,
                    DateCreated = DateTime.Now
                };
                _dataContext.Add(newVnpayInsert);
                await _dataContext.SaveChangesAsync();
                var paymentMethod = response.PaymentMethod;
                var paymentId = response.PaymentId;
                await Checkout(paymentMethod, paymentId);
            }
            else
            {
                TempData["success"] = "Giao dịch thành công!";
                return RedirectToAction("Index", "Cart");
            }
            return View(response);
        }

    }
}
