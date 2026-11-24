using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.API.Extensions;
using TaskHive.Service.DTOs.Requests.UserMembership;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.UserMembershipService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            if (result == null) return NotFound();
            if (!User.IsSelfOrAdmin(result.UserId)) return Forbid();
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<UserMembershipResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllUserMembershipsAsync();
            return Ok(result);
        }

        // Membership của người dùng chỉ được tạo qua thanh toán; tạo/sửa/xoá tay chỉ dành cho Admin.
        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
            if (!User.IsSelfOrAdmin(userId)) return Forbid();

            var list = await _service.GetActiveMembershipsByUserAsync(userId);
            // 200 OK, body là mảng (có thể rỗng nếu user không có membership nào active)
            return Ok(list);
        }
    }
}
