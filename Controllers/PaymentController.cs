using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Models.Vnpay;
using Shopping.Services.Momo;
using Shopping.Services.VnPay;

namespace Shopping.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private IMomoService _momoService;
        public PaymentController(IMomoService momoService, IVnPayService vnPayService)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentMomo(OrderInfoModel model)
        {
            var response = await _momoService.CreatePaymentAsync(model);
            return Redirect(response.PayUrl);
        }
        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response);
        }
        public IActionResult CreatePaymentUrlVnpay(PaymentInfoModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

    }
}
