using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.Service.DTOs.Requests.JobPost;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.JobPostService;
using Microsoft.AspNetCore.Authorization;
using TaskHive.API.Extensions;
using TaskHive.Repository.Repositories.JobPostRepository;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;

        public JobPostController(IJobPostService jobPostService)
        {
            _jobPostService = jobPostService;
        }

        // ✅ Add new endpoint with ratings using Projection
        [HttpGet("paged-with-ratings")]
        [ProducesResponseType(typeof(PagedResult<JobPostWithRatingResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPagedJobPostsWithRatings([FromQuery] JobQueryParam parameters)
        {
            var pagedResult = await _jobPostService.GetJobPostsWithRatingsPagedAsync(parameters);
            if (pagedResult == null || pagedResult.Items == null || !pagedResult.Items.Any())
                return NotFound("No job posts found.");
            return Ok(pagedResult);
        }

        [HttpGet("{jobPostId}")]
        [ProducesResponseType(typeof(JobPostResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetJobPostById(int jobPostId)
        {
            var jobPost = await _jobPostService.GetJobPostByIdAsync(jobPostId);
            return jobPost == null ? NotFound() : Ok(jobPost);
        }

        [HttpGet("employer/{employerId}")]
        [ProducesResponseType(typeof(List<JobPostResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetJobPostsByEmployerId(int employerId)
        {
            var jobPosts = await _jobPostService.GetJobPostsByEmployerIdAsync(employerId);
            return Ok(jobPosts);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<JobPostResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllJobPosts()
        {
            var jobPosts = await _jobPostService.GetAllJobPostsAsync();
            return Ok(jobPosts);
        }

        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(typeof(List<JobPostResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetJobPostsByCategoryId(int categoryId)
        {
            var jobPosts = await _jobPostService.GetJobPostsByCategoryIdAsync(categoryId);
            return Ok(jobPosts);
        }

        [HttpPost]
        [Authorize(Roles = "Client,Admin")]
        [ProducesResponseType(typeof(JobPostResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddJobPost([FromBody] AddJobPostRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Client luôn đăng job dưới tên chính mình; chỉ Admin được chỉ định EmployerId khác.
            if (!User.IsAdmin())
                request.EmployerId = User.GetUserId() ?? 0;

            var (jobPost, error) = await _jobPostService.AddJobPostAsync(request);

            if (error != null || jobPost == null)
                return BadRequest(error ?? "Unknown error occurred.");

            return CreatedAtAction(nameof(GetJobPostById), new { jobPostId = jobPost.JobPostId }, jobPost);
        }

        [HttpPut]
        [ProducesResponseType(typeof(JobPostResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Authorize]
        public async Task<IActionResult> UpdateJobPost([FromBody] UpdateJobPostRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _jobPostService.GetJobPostByIdAsync(request.JobPostId);
            if (existing == null) return NotFound("Job post not found");
            if (!User.IsSelfOrAdmin(existing.EmployerId)) return Forbid();

            var (updatedJobPost, error) = await _jobPostService.UpdateJobPostAsync(request);

            if (error != null)
                return NotFound(error);

            return Ok(updatedJobPost);
        }

        [HttpDelete("{jobPostId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [Authorize]
        public async Task<IActionResult> DeleteJobPost(int jobPostId)
        {
            var existing = await _jobPostService.GetJobPostByIdAsync(jobPostId);
            if (existing == null) return NotFound("Job post not found");
            if (!User.IsSelfOrAdmin(existing.EmployerId)) return Forbid();

            var error = await _jobPostService.DeleteJobPostAsync(jobPostId);
            return error == null ? NoContent() : NotFound(error);
        }


        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<JobPostResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPagedJobPosts([FromQuery] JobQueryParam parameters)
        {
            var pagedResult = await _jobPostService.GetJobPostsPagedAsync(parameters);
            if (pagedResult == null || pagedResult.Items == null || !pagedResult.Items.Any())
                return NotFound("No job posts found.");
            return Ok(pagedResult);
        }
    }
}
