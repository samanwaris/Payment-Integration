using payment_service.Models;
using payment_service.Models.Request;
using payment_service.Models.Response;
using static payment_service.Models.Response.CreatePaymentResponse;

namespace payment_service.Interfaces
{
    public interface ICreatePaymentService
    {
        //Create Payment
        Task<ApiResponse<CreatePaymentResponse.NonSnap>> CreatePaymentNonSnapAsync(CreatePaymentRequest.NonSnap request);
        Task<ApiResponse<CreatePaymentResponse.Snap>> CreatePaymentSnapAsync(CreatePaymentRequest.Snap request, SnapRequestHeader SnapHeader);

        //Payment Settlement
        Task<ApiResponse<PaymentSettlementResponse>> PaymentSettlementAsync(SettlementRequest.NonSnapSettlement request);
        Task<ApiResponse<VirtualAccountData>> PaymentSettlementSnapAsync(SnapRequestHeader header, string rawBody, string httpMethod);
    }
}
