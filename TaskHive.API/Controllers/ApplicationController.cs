// TaskHive.API/Controllers/ApplicationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.API.Extensions;
using TaskHive.Service.DTOs.Requests.Application;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.ApplicationService;
using TaskHive.Service.Services.JobPostService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IJobPostService _jobPostService;

        public ApplicationController(IApplicationService applicationService, IJobPostService jobPostService)
        {
            _applicationService = applicationService;
            _jobPostService = jobPostService;
        }

        /// <summary>Freelancer ứng tuyển, employer của job, hoặc Admin.</summary>
        [HttpGet("{applicationId}")]
        [ProducesResponseType(typeof(ApplicationResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetApplicationById(int applicationId)
        {
            var participants = await _applicationService.GetParticipantsAsync(applicationId);
            if (participants == null) return NotFound();
            if (!CanView(participants.Value)) return Forbid();

            var application = await _applicationService.GetApplicationByIdAsync(applicationId);
            return application == null ? NotFound() : Ok(application);
        }

        /// <summary>Chỉ employer sở hữu job (hoặc Admin) được xem danh sách ứng viên.</summary>
        [HttpGet("jobpost/{jobPostId}")]
        [ProducesResponseType(typeof(List<ApplicationResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetApplicationsByJobPostId(int jobPostId)
        {
            var jobPost = await _jobPostService.GetJobPostByIdAsync(jobPostId);
            if (jobPost == null) return NotFound();
            if (!User.IsSelfOrAdmin(jobPost.EmployerId)) return Forbid();

            var applications = await _applicationService.GetApplicationsByJobPostIdAsync(jobPostId);
            return Ok(applications);
        }

        [HttpGet("freelancer/{freelancerId}")]
        [ProducesResponseType(typeof(List<ApplicationResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetApplicationsByFreelancerId(int freelancerId)
        {
            if (!User.IsSelfOrAdmin(freelancerId)) return Forbid();

            var applications = await _applicationService.GetApplicationsByFreelancerIdAsync(freelancerId);
            return Ok(applications);
        }

        /// <summary>
        /// FreelancerId lấy từ token (giá trị trong form bị bỏ qua). Server kiểm tra trùng và trừ 1 slot.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Freelancer")]
        [RequestSizeLimit(11 * 1024 * 1024)]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddApplication([FromForm] AddApplicationRequestDto applicationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var (application, error) = await _applicationService.AddApplicationAsync(applicationDto, userId.Value);
            if (error != null)
                return BadRequest(error);

            return CreatedAtAction(nameof(GetApplicationById),
                new { applicationId = application?.ApplicationId }, application);
        }

        /// <summary>
        /// Freelancer sở hữu: sửa CoverLetter/BidAmount/CV. Employer của job: đổi Status. Admin: cả hai.
        /// </summary>
        [HttpPut]
        [RequestSizeLimit(11 * 1024 * 1024)]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateApplication([FromForm] UpdateApplicationRequestDto applicationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var participants = await _applicationService.GetParticipantsAsync(applicationDto.ApplicationId);
            if (participants == null) return NotFound();

            var userId = User.GetUserId();
            var isAdmin = User.IsAdmin();
            var canEditContent = isAdmin || userId == participants.Value.FreelancerId;
            var canEditStatus = isAdmin || userId == participants.Value.EmployerId;
            if (!canEditContent && !canEditStatus) return Forbid();

            var (application, error) = await _applicationService.UpdateApplicationAsync(
                applicationDto, canEditContent, canEditStatus);
            if (error != null)
                return BadRequest(error);
            if (application == null)
                return NotFound();

            return Ok(application);
        }

        [HttpDelete("{applicationId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteApplication(int applicationId)
        {
            var participants = await _applicationService.GetParticipantsAsync(applicationId);
            if (participants == null) return NotFound();
            if (!User.IsSelfOrAdmin(participants.Value.FreelancerId)) return Forbid();

            var error = await _applicationService.DeleteApplicationAsync(applicationId);
            if (error != null) return BadRequest(error);
            return NoContent();
        }

        private bool CanView((int FreelancerId, int EmployerId) p)
            => User.IsAdmin() || User.GetUserId() is int me && (me == p.FreelancerId || me == p.EmployerId);
    }
}
