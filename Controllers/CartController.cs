using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping.Models;
using Shopping.Models.ViewModels;
using Shopping.Repository;
using System.Collections.Immutable;

namespace Shopping.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index()
        {
            List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;

            var coupon_code = Request.Cookies["CouponTitle"];

            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            CartItemViewModel cartVM = new CartItemViewModel()
            {
                CartItems = cartItem,
                GrandTotal = cartItem.Sum(x => x.Quantity * x.Price),
                ShippingCost = shippingPrice,
                CouponCode = coupon_code
            };
            return View(cartVM);
        }

        public ActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }

        public async Task<IActionResult> Add(int Id)
        {
            ProductsModel products = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = cart.Where(c => c.ProductID == Id).FirstOrDefault();

            if (cartItems == null)
            {
                cart.Add(new CartItemModel(products));
            }
            else
            {
                cartItems.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Add item to cart successfully!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

            CartItemModel cartItem = cart.Where(c => c.ProductID == Id).FirstOrDefault();

            if (cartItem != null)
            {
                cartItem.Quantity--;

                if (cartItem.Quantity <= 0)
                {
                    cart.RemoveAll(p => p.ProductID == Id);
                }

                if (cart.Count == 0)
                {
                    HttpContext.Session.Remove("Cart");
                }
                else
                {
                    HttpContext.Session.SetJson("Cart", cart);
                }

                TempData["success"] = "Decrease successfully!";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int Id)
        {
            ProductsModel product = await _dataContext.Products.Where(p => p.Id == Id).FirstOrDefaultAsync();
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductID == Id).FirstOrDefault();

            if (cartItem.Quantity >= 1 && product.Quantity > cartItem.Quantity)
            {
                ++cartItem.Quantity;
            }
            else
            {
                cartItem.Quantity = product.Quantity;
                TempData["success"] = "Đã đạt số lượng tối đa!";
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Increase successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.ProductID == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Remove successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear(int Id)
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear all items  successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Cart/GetShipping")]
        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string quan, string tinh, string phuong)
        {

            var existingShipping = await _dataContext.Shipping.FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

            decimal shippingPrice = 0; // Set mặc định giá tiền

            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                //Set mặc định giá tiền nếu ko tìm thấy
                shippingPrice = 50000;
            }
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true // using HTTPS
                };

                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding shipping price cookie: {ex.Message}");
            }
            return Json(new { shippingPrice });
        }

        [HttpGet]
        [Route("Cart/DeleteShipping")]
        public IActionResult DeleteShipping()
        {
            Response.Cookies.Delete("ShippingPrice");
            return RedirectToAction("Index","Cart");
        }

        [HttpPost]
        [Route("Cart/GetCoupon")]
        public async Task<IActionResult> GetCoupon(CouponModel couponModel, string coupon_value)
        {
            var validCoupon = await _dataContext.Coupon
                .FirstOrDefaultAsync(x => x.Name == coupon_value);

            if (validCoupon == null)
            {
                return Ok(new { success = false, message = "Coupon not existed" });
            }

            string couponTitle = validCoupon.Name + " | " + validCoupon?.Description;

            if (couponTitle != null)
            {
                TimeSpan remainingTime = validCoupon.DateEnd - DateTime.Now;
                int daysRemaining = remainingTime.Days;

                if (daysRemaining >= 0)
                {
                    try
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Strict // Kiểm tra tính tương thích trình duyệt
                        };

                        Response.Cookies.Append("CouponTitle", couponTitle, cookieOptions);
                        return Ok(new { success = true, message = "Coupon applied successfully" });
                    }
                    catch (Exception ex)
                    {
                        //trả về lỗi 
                        Console.WriteLine($"Error adding apply coupon cookie: {ex.Message}");
                        return Ok(new { success = false, message = "Coupon applied failed" });
                    }
                }
                else
                {

                    return Ok(new { success = false, message = "Coupon has expired" });
                }

            }
            else
            {
                return Ok(new { success = false, message = "Coupon not existed" });
            }
        }
    }
}
