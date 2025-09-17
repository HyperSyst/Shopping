using Shopping.Models;
using Shopping.Models.Momo;

namespace Shopping.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
        MomoExcuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
