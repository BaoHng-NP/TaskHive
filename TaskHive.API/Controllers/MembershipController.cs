using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.Service.DTOs.Requests.Membership;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.MembershipService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpGet("{membershipId}")]
        [ProducesResponseType(typeof(MembershipResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMembershipById(int membershipId)
        {
            var membership = await _membershipService.GetMembershipByIdAsync(membershipId);
            return membership == null ? NotFound() : Ok(membership);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<MembershipResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllMemberships()
        {
            var memberships = await _membershipService.GetAllMembershipsAsync();
            return Ok(memberships);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MembershipResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddMembership(AddMembershipRequestDto membershipDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (membership, error) = await _membershipService.AddMembershipAsync(membershipDto);
            if (error != null) return BadRequest(error);

            return CreatedAtAction(nameof(GetMembershipById), new { membershipId = membership?.MembershipId }, membership);
        }

        [HttpPut]
        [ProducesResponseType(typeof(MembershipResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateMembership(UpdateMembershipRequestDto membershipDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (membership, error) = await _membershipService.UpdateMembershipAsync(membershipDto);
            if (error != null) return BadRequest(error);
            if (membership == null) return NotFound();

            return Ok(membership);
        }

        [HttpDelete("{membershipId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteMembership(int membershipId)
        {
            var error = await _membershipService.DeleteMembershipAsync(membershipId);
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }
}
