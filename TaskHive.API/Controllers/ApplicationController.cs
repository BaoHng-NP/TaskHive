// TaskHive.API/Controllers/ApplicationController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.Service.DTOs.Requests.Application;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.ApplicationService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet("{applicationId}")]
        [ProducesResponseType(typeof(ApplicationResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetApplicationById(int applicationId)
        {
            var application = await _applicationService.GetApplicationByIdAsync(applicationId);
            return application == null ? NotFound() : Ok(application);
        }

        [HttpGet("jobpost/{jobPostId}")]
        [ProducesResponseType(typeof(List<ApplicationResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetApplicationsByJobPostId(int jobPostId)
        {
            var applications = await _applicationService.GetApplicationsByJobPostIdAsync(jobPostId);
            return Ok(applications);
        }

        [HttpGet("freelancer/{freelancerId}")]
        [ProducesResponseType(typeof(List<ApplicationResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetApplicationsByFreelancerId(int freelancerId)
        {
            var applications = await _applicationService.GetApplicationsByFreelancerIdAsync(freelancerId);
            return Ok(applications);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddApplication([FromForm] AddApplicationRequestDto applicationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (application, error) = await _applicationService.AddApplicationAsync(applicationDto);
            if (error != null)
                return BadRequest(error);

            return CreatedAtAction(nameof(GetApplicationById),
                new { applicationId = application?.ApplicationId }, application);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateApplication([FromForm] UpdateApplicationRequestDto applicationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (application, error) = await _applicationService.UpdateApplicationAsync(applicationDto);
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
            var error = await _applicationService.DeleteApplicationAsync(applicationId);
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }
}