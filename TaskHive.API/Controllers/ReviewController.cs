using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskHive.Service.DTOs.Requests.Review;
using TaskHive.Service.Services.ReviewService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        //Get reviews tôi đã viết cho job post này
        [HttpGet("my-reviews/job-post/{jobPostId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyReviewsForJobPost(int jobPostId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user token" });

            var reviews = await _reviewService.GetMyReviewsForJobPostAsync(jobPostId, userId);
            return Ok(new { jobPostId, myReviews = reviews });
        }

        //Get reviews tôi nhận được trong job post này
        [HttpGet("received-reviews/job-post/{jobPostId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetReceivedReviewsForJobPost(int jobPostId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user token" });

            var reviews = await _reviewService.GetReceivedReviewsForJobPostAsync(jobPostId, userId);
            return Ok(new { jobPostId, receivedReviews = reviews });
        }

        //Get tất cả reviews tôi nhận được
        [HttpGet("my-received-reviews")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllMyReceivedReviews()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user token" });

            var reviews = await _reviewService.GetAllReceivedReviewsAsync(userId);

            //Tính thống kê
            var stats = new
            {
                TotalReviews = reviews.Count,
                AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 2) : 0,
                RatingDistribution = reviews.GroupBy(r => r.Rating)
                    .Select(g => new { Rating = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Rating)
                    .ToList()
            };

            return Ok(new { reviews, statistics = stats });
        }

        //Get tất cả reviews tôi đã viết
        [HttpGet("my-given-reviews")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllMyGivenReviews()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user token" });

            var reviews = await _reviewService.GetAllGivenReviewsAsync(userId);
            return Ok(new { givenReviews = reviews });
        }

        //Get reviews của user khác (public)
        [HttpGet("user/{userId}/received-reviews")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserReceivedReviews(int userId)
        {
            var reviews = await _reviewService.GetAllReceivedReviewsAsync(userId);

            var stats = new
            {
                TotalReviews = reviews.Count,
                AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 2) : 0,
                RatingDistribution = reviews.GroupBy(r => r.Rating)
                    .Select(g => new { Rating = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Rating)
                    .ToList()
            };

            return Ok(new { userId, reviews, statistics = stats });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReview(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);

            if (review == null)
                return NotFound(new { message = "Review not found" });

            return Ok(review);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateReview([FromBody] ReviewRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(reviewerIdClaim) || !int.TryParse(reviewerIdClaim, out var reviewerId))
                return Unauthorized(new { message = "Invalid user token" });

            var review = await _reviewService.CreateReviewAsync(request, reviewerId);

            if (review == null)
                return BadRequest(new
                {
                    message = "Failed to create review. Please check: 1) Job must be finished, 2) You haven't reviewed this person for this job, 3) You cannot review your own job post, 4) You cannot review yourself."
                });

            return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, review);
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(reviewerIdClaim) || !int.TryParse(reviewerIdClaim, out var reviewerId))
                return Unauthorized(new { message = "Invalid user token" });

            var review = await _reviewService.UpdateReviewAsync(id, request, reviewerId);

            if (review == null)
                return NotFound(new { message = "Review not found or unauthorized" });

            return Ok(review);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var reviewerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(reviewerIdClaim) || !int.TryParse(reviewerIdClaim, out var reviewerId))
                return Unauthorized(new { message = "Invalid user token" });

            var success = await _reviewService.DeleteReviewAsync(id, reviewerId);

            if (!success)
                return NotFound(new { message = "Review not found or unauthorized" });

            return Ok(new { success = true, message = "Review deleted successfully" });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        //Create platform review
        [HttpPost("platform")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreatePlatformReview([FromBody] PlatformReviewRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(reviewerIdClaim) || !int.TryParse(reviewerIdClaim, out var reviewerId))
                return Unauthorized(new { message = "Invalid user token" });

            var (review, errorMessage) = await _reviewService.CreatePlatformReviewAsync(request, reviewerId);

            if (review == null)
                return Conflict(new
                {
                    message = errorMessage ?? "Failed to create platform review.",
                    code = "PLATFORM_REVIEW_FAILED"
                });

            return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, new
            {
                review,
                message = "Platform review created successfully"
            });
        }

        // Get platform reviews
        [HttpGet("platform")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPlatformReviews()
        {
            var reviews = await _reviewService.GetPlatformReviewsAsync();
            return Ok(new
            {
                platformReviews = reviews,
                count = reviews.Count,
                message = "Platform reviews retrieved successfully"
            });
        }

        //Get platform reviews with statistics
        [HttpGet("platform/stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPlatformReviewsWithStats()
        {
            var result = await _reviewService.GetPlatformReviewsWithStatsAsync();
            return Ok(result);
        }
    }
}
