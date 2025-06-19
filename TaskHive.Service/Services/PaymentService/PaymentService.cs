using AutoMapper;
using Net.payOS;
using Net.payOS.Errors;
using Net.payOS.Types;
using System.Linq;
using TaskHive.Repository.Entities;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.Payment;
using TaskHive.Service.DTOs.Responses;


namespace TaskHive.Service.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PayOS _payOs;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, PayOS payOs)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _payOs = payOs;
        }

        public async Task<(PaymentResponseDto? payment, string? error)> AddPaymentWithMembershipAsync(AddPaymentWithMembershipRequestDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            payment.SlotPurchaseId = null;

            var success = await _unitOfWork.Payments.AddPaymentAsync(payment);
            if (!success) return (null, "Failed to create payment");

            await _unitOfWork.SaveChangesAsync();

            return (_mapper.Map<PaymentResponseDto>(payment), null);
        }

        public async Task<(PaymentResponseDto? payment, string? error)> AddPaymentWithSlotAsync(AddPaymentWithSlotRequestDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            payment.MembershipId = null;

            var success = await _unitOfWork.Payments.AddPaymentAsync(payment);
            if (!success) return (null, "Failed to create payment");

            await _unitOfWork.SaveChangesAsync();

            return (_mapper.Map<PaymentResponseDto>(payment), null);
        }

        public async Task<List<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllPaymentsAsync();
            return _mapper.Map<List<PaymentResponseDto>>(payments);
        }

        public async Task<CreatePaymentResult> CreatePaymentLinkAsync(PaymentDataDto dto)
        {
            // validate description
            var desc = dto.Description.Length > 25
                ? dto.Description.Substring(0, 25)
                : dto.Description;

            var items = dto.Items
                           .Select(i => new ItemData(i.Name, i.Quantity, i.Price))
                           .ToList();

            var payRequest = new PaymentData(
                orderCode: dto.OrderCode,
                amount: dto.Amount,
                description: desc,
                items: items,
                cancelUrl: dto.CancelUrl,
                returnUrl: dto.ReturnUrl
            );

            try
            {
                return await _payOs.createPaymentLink(payRequest);
            }
            catch (PayOSError ex)
            {
                // bạn có thể log ex.Message, trả về null hoặc throw tiếp
                throw new ApplicationException($"Tạo payment link thất bại: {ex.Message}");
            }
        }

        public async Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderCode)
        {
            // Gọi SDK của PayOs để fetch thông tin
            var info = await _payOs.getPaymentLinkInformation(orderCode);
            return info;
        }
    }
}
