using TaskHive.Service.DTOs.Requests.Payment;
using TaskHive.Service.DTOs.Responses;
using Net.payOS;
using Net.payOS.Types;

namespace TaskHive.Service.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<(PaymentResponseDto? payment, string? error)> AddPaymentWithMembershipAsync(AddPaymentWithMembershipRequestDto dto);
        Task<(PaymentResponseDto? payment, string? error)> AddPaymentWithSlotAsync(AddPaymentWithSlotRequestDto dto);
        Task<List<PaymentResponseDto>> GetAllPaymentsAsync();
        Task<CreatePaymentResult> CreatePaymentLinkAsync(PaymentDataDto dto);
        Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderCode);
    }
}
