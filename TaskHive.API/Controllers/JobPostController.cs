using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.Service.DTOs.Requests.JobPost;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.JobPostService;
using Microsoft.AspNetCore.Authorization;
using TaskHive.Repository.Repositories.JobPostRepository;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Bỏ comment để bật xác thực nếu có dùng JWT
    public class JobPostController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;

        public JobPostController(IJobPostService jobPostService)
        {
            _jobPostService = jobPostService;
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
        [ProducesResponseType(typeof(JobPostResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddJobPost([FromBody] AddJobPostRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (jobPost, error) = await _jobPostService.AddJobPostAsync(request);

            if (error != null || jobPost == null)
                return BadRequest(error ?? "Unknown error occurred.");

            return CreatedAtAction(nameof(GetJobPostById), new { jobPostId = jobPost.JobPostId }, jobPost);
        }

        [HttpPut]
        [ProducesResponseType(typeof(JobPostResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateJobPost([FromBody] UpdateJobPostRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (updatedJobPost, error) = await _jobPostService.UpdateJobPostAsync(request);

            if (error != null)
                return NotFound(error);

            return Ok(updatedJobPost);
        }

        [HttpDelete("{jobPostId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteJobPost(int jobPostId)
        {
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
