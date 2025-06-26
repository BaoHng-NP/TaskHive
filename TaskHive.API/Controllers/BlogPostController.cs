using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskHive.Service.DTOs.Requests.BlogPost;
using TaskHive.Service.Services.BlogPostService;
using TaskHive.Repository.Repositories.BlogPostRepository;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;

        public BlogPostController(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedBlogPosts([FromQuery] PostQueryParam parameters)
        {
            var result = await _blogPostService.GetPagedBlogPostsAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBlogPost(int id)
        {
            var blogPost = await _blogPostService.GetBlogPostByIdAsync(id);

            if (blogPost == null)
                return NotFound(new { message = "Blog post not found" });

            return Ok(blogPost);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateBlogPost([FromBody] BlogPostRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(authorIdClaim) || !int.TryParse(authorIdClaim, out var authorId))
                return Unauthorized(new { message = "Invalid user token" });

            var blogPost = await _blogPostService.CreateBlogPostAsync(request, authorId);

            if (blogPost == null)
                return BadRequest(new { message = "Failed to create blog post" });

            return CreatedAtAction(nameof(GetBlogPost), new { id = blogPost.BlogpostId }, blogPost);
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBlogPost(int id, [FromBody] BlogPostRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(authorIdClaim) || !int.TryParse(authorIdClaim, out var authorId))
                return Unauthorized(new { message = "Invalid user token" });

            var updateRequest = new UpdateBlogPostDto
            {
                Id = id,
                Title = request.Title,
                Content = request.Content,
                PublishedAt = request.PublishedAt,
                Status = request.Status,
                IsDeleted = request.IsDeleted
            };

            var blogPost = await _blogPostService.UpdateBlogPostAsync(updateRequest, authorId);

            if (blogPost == null)
                return NotFound(new { message = "Blog post not found or unauthorized" });

            return Ok(blogPost);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var authorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(authorIdClaim) || !int.TryParse(authorIdClaim, out var authorId))
                return Unauthorized(new { message = "Invalid user token" });

            var success = await _blogPostService.DeleteBlogPostAsync(id, authorId);

            if (!success)
                return NotFound(new { message = "Blog post not found or unauthorized" });

            return Ok(new { success = true, message = "Blog post deleted successfully" });
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await _blogPostService.GetAllBlogPostsAsync();
            return Ok(blogPosts);
        }
    }
}