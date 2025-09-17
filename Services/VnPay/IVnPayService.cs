using Shopping.Models.Vnpay;

namespace Shopping.Services.VnPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInfoModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
