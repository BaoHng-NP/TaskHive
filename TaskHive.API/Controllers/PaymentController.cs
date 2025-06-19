using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.Service.DTOs.Requests.Payment;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.PaymentService;
using Net.payOS;
using Net.payOS.Types;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("membership")]
        [ProducesResponseType(typeof(PaymentResponseDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddMembershipPayment(AddPaymentWithMembershipRequestDto dto)
        {
            var (payment, error) = await _paymentService.AddPaymentWithMembershipAsync(dto);
            if (error != null) return BadRequest(error);
            return CreatedAtAction(nameof(GetAllPayments), new { }, payment);
        }

        [HttpPost("slot")]
        [ProducesResponseType(typeof(PaymentResponseDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddSlotPayment(AddPaymentWithSlotRequestDto dto)
        {
            var (payment, error) = await _paymentService.AddPaymentWithSlotAsync(dto);
            if (error != null) return BadRequest(error);
            return CreatedAtAction(nameof(GetAllPayments), new { }, payment);
        }

        [HttpGet]
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
