using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.API.Extensions;
using TaskHive.Service.DTOs.Requests.Payment;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.PaymentService;
using Net.payOS.Types;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // TODO (giai đoạn 2): endpoint này vẫn tin Amount/Status do client gửi.
        // Phải thay bằng webhook PayOS có xác minh chữ ký trước khi cấp membership/slot.
        [HttpPost("membership")]
        [ProducesResponseType(typeof(PaymentResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddMembershipPayment([FromBody] AddPaymentWithMembershipRequestDto dto)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            dto.UserId = userId.Value; // không cho mua hộ / ghi payment cho người khác

            var (payment, error) = await _paymentService.AddPaymentWithMembershipAsync(dto);
            if (error != null)
                return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetAllPayments), new { }, payment);
        }

        // TODO (giai đoạn 2): như trên — chưa xác minh với PayOS.
        [HttpPost("slot")]
        [ProducesResponseType(typeof(PaymentResponseDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddSlotPayment(AddPaymentWithSlotRequestDto dto)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();
            dto.UserId = userId.Value;

            var (payment, error) = await _paymentService.AddPaymentWithSlotAsync(dto);
            if (error != null) return BadRequest(error);
            return CreatedAtAction(nameof(GetAllPayments), new { }, payment);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<PaymentResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

        [HttpPost("create-link")]
        public async Task<ActionResult<CreatePaymentResult>> CreateLink([FromBody] PaymentDataDto request)
        {
            var result = await _paymentService.CreatePaymentLinkAsync(request);
            return Ok(result);
        }

        [HttpGet("status/{orderCode}")]
        public async Task<IActionResult> GetStatus(long orderCode)
        {
            var info = await _paymentService.GetPaymentLinkInformationAsync(orderCode);
            return Ok(info);
        }
    }
}
