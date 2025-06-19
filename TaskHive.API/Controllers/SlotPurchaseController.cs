using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.Service.DTOs.Requests.SlotPurchase;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.SlotPurchaseService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotPurchaseController : ControllerBase
    {
        private readonly ISlotPurchaseService _service;

        public SlotPurchaseController(ISlotPurchaseService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SlotPurchaseResponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddSlotPurchaseRequestDto dto)
        {
            var (result, error) = await _service.AddAsync(dto);
            if (error != null) return BadRequest(error);
            return CreatedAtAction(nameof(GetById), new { id = result!.SlotPurchaseId }, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateSlotPurchaseRequestDto dto)
        {
            var (result, error) = await _service.UpdateAsync(dto);
            if (error != null) return BadRequest(error);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var error = await _service.DeleteAsync(id);
            return error != null ? BadRequest(error) : NoContent();
        }
    }
}
