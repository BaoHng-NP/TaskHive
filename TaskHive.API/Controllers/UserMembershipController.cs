using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.Service.DTOs.Requests.UserMembership;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.UserMembershipService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMembershipController : ControllerBase
    {
        private readonly IUserMembershipService _service;

        public UserMembershipController(IUserMembershipService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserMembershipResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetUserMembershipByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UserMembershipResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllUserMembershipsAsync();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserMembershipResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Add(AddUserMembershipRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (result, error) = await _service.AddUserMembershipAsync(dto);
            if (error != null) return BadRequest(error);

            return CreatedAtAction(nameof(GetById), new { id = result?.UserMembershipId }, result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserMembershipResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(UpdateUserMembershipRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (result, error) = await _service.UpdateUserMembershipAsync(dto);
            if (error != null) return BadRequest(error);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var error = await _service.DeleteUserMembershipAsync(id);
            if (error != null) return BadRequest(error);
            return NoContent();
        }
        [HttpGet("active/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<UserMembershipResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetActiveByUser([FromRoute] int userId)
        {
            var list = await _service.GetActiveMembershipsByUserAsync(userId);

            // Trả về 200 OK, body là mảng (có thể rỗng nếu user không có membership nào active)
            return Ok(list);
        }
    }
}
