using AutoMapper;
using Net.payOS;
using Net.payOS.Errors;
using Net.payOS.Types;
using System.Linq;
using TaskHive.Repository.Entities;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.Payment;
using TaskHive.Service.DTOs.Requests.UserMembership;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.UserMembershipService;
using TaskHive.Service.Services.UserService;


namespace TaskHive.Service.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PayOS _payOs;
        private readonly IUserMembershipService _userMembershipService;
        private readonly IUserService _userService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, PayOS payOs, IUserMembershipService userMembershipService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _payOs = payOs;
            _userMembershipService = userMembershipService;
            _userService = userService;
        }

        public async Task<(PaymentResponseDto? payment, string? error)> AddPaymentWithMembershipAsync(AddPaymentWithMembershipRequestDto dto)
        {
            // 1. Tạo payment như bình thường
            var payment = _mapper.Map<Payment>(dto);
            payment.SlotPurchaseId = null;
            payment.SlotQuantity = null;

            var paymentCreated = await _unitOfWork.Payments.AddPaymentAsync(payment);
            if (!paymentCreated)
                return (null, "Failed to create payment");

            await _unitOfWork.SaveChangesAsync();

            // 2. Xử lý UserMembership
            // a) Lấy danh sách memberships đang active của user
            var existingMemberships = await _unitOfWork.UserMemberships
                .FindAsync(um => um.UserId == dto.UserId && um.IsActive);

            // b) Nếu có thì đặt isActive = false cho tất cả bản ghi cũ
            if (existingMemberships.Any())
            {
                foreach (var old in existingMemberships)
                {
                    old.IsActive = false;
                    await _unitOfWork.UserMemberships.UpdateUserMembershipAsync(old);
                }
                // sau khi update, SaveChangesAsync
                await _unitOfWork.SaveChangesAsync();
            }

            // c) Tạo bản ghi mới cho UserMembership
            var userMembershipDto = new AddUserMembershipRequestDto
            {
                UserId = dto.UserId,
                MembershipId = dto.MembershipId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1), // hoặc tính theo membership.Duration
                RemainingSlots = 0,                       // hoặc số connects mặc định
                IsActive = true
            };

            var (addedUm, umError) = await _userMembershipService.AddUserMembershipAsync(userMembershipDto);
            if (umError != null)
                return (null, $"Payment created but failed to add membership: {umError}");

            // 3. Map và trả về kết quả
            var resultDto = _mapper.Map<PaymentResponseDto>(payment);
            return (resultDto, null);
        }


        public async Task<(PaymentResponseDto? payment, string? error)> AddPaymentWithSlotAsync(AddPaymentWithSlotRequestDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            payment.MembershipId = null;
            var slotQuantity = (int)(dto.SlotQuantity ?? 0);

            var success = await _unitOfWork.Payments.AddPaymentAsync(payment);
            if (!success) return (null, "Failed to create payment");
            var updatedSlotPurchase = await _userService.UpdateRemainingSlotAsync(dto.UserId, slotQuantity);

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
            // ✅ Tạo OrderCode duy nhất — ví dụ dùng timestamp + random
            long orderCode = GenerateOrderCode();
            // validate description
            var desc = dto.Description.Length > 25
                ? dto.Description.Substring(0, 25)
                : dto.Description;

            var items = dto.Items
                           .Select(i => new ItemData(i.Name, i.Quantity, i.Price))
                           .ToList();

            var payRequest = new PaymentData(
                orderCode: orderCode,
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
        private long GenerateOrderCode()
        {
            // Cách đơn giản: Timestamp (milli) + 3 chữ số random (an toàn trong phần lớn các case)
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 13 chữ số
            var random = new Random().Next(100, 999); // 3 chữ số
            return long.Parse($"{timestamp}{random}");
        }

    }
}
