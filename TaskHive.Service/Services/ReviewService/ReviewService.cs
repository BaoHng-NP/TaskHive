// ReviewService.cs
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.Review;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const int PLATFORM_REVIEWEE_ID = 19; 
        private const int PLATFORM_JOB_POST_ID = 13; 

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ReviewResponseDto>> GetAllReviewsAsync()
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetAllAsync();
                return _mapper.Map<List<ReviewResponseDto>>(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get all reviews failed: {ex.Message}");
                return new List<ReviewResponseDto>();
            }
        }

        public async Task<ReviewResponseDto?> GetReviewByIdAsync(int reviewId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                return review == null ? null : _mapper.Map<ReviewResponseDto>(review);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get review by id failed: {ex.Message}");
                return null;
            }
        }

        public async Task<List<ReviewResponseDto>> GetMyReviewsForJobPostAsync(int jobPostId, int reviewerId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetByPostIdAndReviewerIdAsync(jobPostId, reviewerId);
                return _mapper.Map<List<ReviewResponseDto>>(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get my reviews for job post failed: {ex.Message}");
                return new List<ReviewResponseDto>();
            }
        }

        public async Task<List<ReviewResponseDto>> GetReceivedReviewsForJobPostAsync(int jobPostId, int revieweeId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetByPostIdAndRevieweeIdAsync(jobPostId, revieweeId);
                return _mapper.Map<List<ReviewResponseDto>>(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get received reviews for job post failed: {ex.Message}");
                return new List<ReviewResponseDto>();
            }
        }

        public async Task<List<ReviewResponseDto>> GetAllReceivedReviewsAsync(int revieweeId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetByRevieweeIdAsync(revieweeId);
                return _mapper.Map<List<ReviewResponseDto>>(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get all received reviews failed: {ex.Message}");
                return new List<ReviewResponseDto>();
            }
        }

        public async Task<List<ReviewResponseDto>> GetAllGivenReviewsAsync(int reviewerId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetByReviewerIdAsync(reviewerId);
                return _mapper.Map<List<ReviewResponseDto>>(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get all given reviews failed: {ex.Message}");
                return new List<ReviewResponseDto>();
            }
        }

        public async Task<ReviewResponseDto?> CreateReviewAsync(ReviewRequestDto request, int reviewerId)
        {
            try
            {
                //  Business Rule 1: Kiểm tra đã review chưa
                var existingReview = await _unitOfWork.Reviews.ExistsAsync(reviewerId, request.RevieweeId, request.JobPostId);
                if (existingReview)
                {
                    Console.WriteLine("You have already reviewed this person for this job post");
                    return null;
                }

                //  Business Rule 2: Không thể review chính mình
                if (reviewerId == request.RevieweeId)
                {
                    Console.WriteLine("Cannot review yourself");
                    return null;
                }

                //  Business Rule 3: Không được review cho job post mình đăng
                //var jobPost = await _unitOfWork.JobPosts.GetJobPostByIdAsync(request.JobPostId);
                //if (jobPost == null)
                //{
                //    Console.WriteLine("Job post not found");
                //    return null;
                //}

                //if (jobPost.EmployerId == reviewerId)
                //{
                //    Console.WriteLine("Cannot review your own job post");
                //    return null;
                //}

                //  Business Rule 4: Application phải ở trạng thái "Finished"
                var canReview = await CanUserReviewAsync(reviewerId, request.RevieweeId, request.JobPostId);
                if (!canReview)
                {
                    Console.WriteLine("You can only review after the job is finished");
                    return null;
                }

                var review = _mapper.Map<Review>(request);
                review.ReviewerId = reviewerId;
                review.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Reviews.CreateAsync(review);
                await _unitOfWork.SaveChangesAsync();

                // Get created review with includes
                var createdReview = await _unitOfWork.Reviews.GetByIdAsync(review.ReviewId);
                return _mapper.Map<ReviewResponseDto>(createdReview);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create review failed: {ex.Message}");
                return null;
            }
        }

        private async Task<bool> CanUserReviewAsync(int reviewerId, int revieweeId, int jobPostId)
        {
            try
            {
                var jobPost = await _unitOfWork.JobPosts.GetJobPostByIdAsync(jobPostId);
                if (jobPost == null) return false;

                // Case 1: Reviewer là client (employer) của job post
                if (reviewerId == jobPost.EmployerId)
                {
                    // Client có thể review freelancer khi job finished
                    var application = await _unitOfWork.Applications.GetByJobPostAndFreelancerAsync(jobPostId, revieweeId);
                    return application != null && application.Status?.ToLower() == "finished";
                }

                // Case 2: Reviewer là freelancer
                var reviewerApplication = await _unitOfWork.Applications.GetByJobPostAndFreelancerAsync(jobPostId, reviewerId);
                if (reviewerApplication != null && reviewerApplication.Status?.ToLower() == "finished")
                {
                    // Freelancer có thể review client (employer) khi job finished
                    return revieweeId == jobPost.EmployerId;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Check review permission failed: {ex.Message}");
                return false;
            }
        }

        public async Task<ReviewResponseDto?> UpdateReviewAsync(int reviewId, UpdateReviewRequestDto request, int reviewerId)
        {
            try
            {
                var existingReview = await _unitOfWork.Reviews.GetByIdAsync(reviewId);

                if (existingReview == null || existingReview.ReviewerId != reviewerId)
                    return null;

                // Update properties
                existingReview.Rating = request.Rating;
                existingReview.Comment = request.Comment;

                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<ReviewResponseDto>(existingReview);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update review failed: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int reviewerId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);

                if (review == null || review.ReviewerId != reviewerId)
                    return false;

                var success = await _unitOfWork.Reviews.DeleteAsync(reviewId);
                if (success)
                {
                    await _unitOfWork.SaveChangesAsync();
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete review failed: {ex.Message}");
                return false;
            }
        }

        public async Task<(ReviewResponseDto? review, string? errorMessage)> CreatePlatformReviewAsync(PlatformReviewRequestDto request, int reviewerId)
        {
            try
            {
                // Check if user already reviewed platform
                var existingReview = await _unitOfWork.Reviews.GetPlatformReviewByUserAsync(reviewerId, PLATFORM_REVIEWEE_ID);
                if (existingReview != null)
                {
                    return (null, "You have already reviewed the platform.");
                }

                // Check if platform admin exists (revieweeId = 16)
                var platformUser = await _unitOfWork.Users.GetByIdAsync(PLATFORM_REVIEWEE_ID);
                if (platformUser == null)
                {
                    return (null, "Platform admin account not found.");
                }

                // Cannot review yourself (if user is admin)
                if (reviewerId == PLATFORM_REVIEWEE_ID)
                {
                    return (null, "You cannot review yourself.");
                }

                var review = new Review
                {
                    ReviewerId = reviewerId,
                    RevieweeId = PLATFORM_REVIEWEE_ID,
                    JobPostId = PLATFORM_JOB_POST_ID, 
                    Rating = request.Rating,
                    Comment = request.Comment,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _unitOfWork.Reviews.CreateAsync(review);
                await _unitOfWork.SaveChangesAsync();

                // Get complete review with related data
                var createdReview = await _unitOfWork.Reviews.GetByIdAsync(review.ReviewId);
                return (_mapper.Map<ReviewResponseDto>(createdReview), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create platform review failed: {ex.Message}");
                return (null, "An error occurred while creating platform review.");
            }
        }

        // NEW: Get platform reviews
        public async Task<List<ReviewResponseDto>> GetPlatformReviewsAsync()
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetPlatformReviewsAsync(PLATFORM_REVIEWEE_ID);
                return _mapper.Map<List<ReviewResponseDto>>(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get platform reviews failed: {ex.Message}");
                return new List<ReviewResponseDto>();
            }
        }

        // NEW: Get platform reviews with statistics
        public async Task<object> GetPlatformReviewsWithStatsAsync()
        {
            try
            {
                var reviews = await GetPlatformReviewsAsync();

                var stats = new
                {
                    TotalReviews = reviews.Count,
                    AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 2) : 0,
                    RatingDistribution = reviews.GroupBy(r => r.Rating)
                        .Select(g => new { Rating = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Rating)
                        .ToList(),
                    RecentReviews = reviews.Take(10).ToList()
                };

                return new
                {
                    reviews,
                    statistics = stats,
                    platformInfo = new
                    {
                        RevieweeId = PLATFORM_REVIEWEE_ID,
                        Name = "TaskHive Platform",
                        Description = "Reviews about TaskHive platform and services"
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get platform reviews with stats failed: {ex.Message}");
                return new
                {
                    reviews = new List<ReviewResponseDto>(),
                    statistics = new
                    {
                        TotalReviews = 0,
                        AverageRating = 0,
                        RatingDistribution = new List<object>(),
                        RecentReviews = new List<ReviewResponseDto>()
                    },
                    error = "Failed to load platform reviews"
                };
            }
        }
    }
}
