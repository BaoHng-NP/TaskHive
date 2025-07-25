using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs.Requests.Review;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.ReviewService
{
    public interface IReviewService
    {
        Task<List<ReviewResponseDto>> GetAllReviewsAsync();
        Task<ReviewResponseDto?> GetReviewByIdAsync(int reviewId);

        // ✅ APIs theo yêu cầu
        Task<List<ReviewResponseDto>> GetMyReviewsForJobPostAsync(int jobPostId, int reviewerId);
        Task<List<ReviewResponseDto>> GetReceivedReviewsForJobPostAsync(int jobPostId, int revieweeId);
        Task<List<ReviewResponseDto>> GetAllReceivedReviewsAsync(int revieweeId);
        Task<List<ReviewResponseDto>> GetAllGivenReviewsAsync(int reviewerId);

        Task<ReviewResponseDto?> CreateReviewAsync(ReviewRequestDto request, int reviewerId);
        Task<ReviewResponseDto?> UpdateReviewAsync(int reviewId, UpdateReviewRequestDto request, int reviewerId);
        Task<bool> DeleteReviewAsync(int reviewId, int reviewerId);

        //Platform reviews
        Task<(ReviewResponseDto? review, string? errorMessage)> CreatePlatformReviewAsync(PlatformReviewRequestDto request, int reviewerId);
        Task<List<ReviewResponseDto>> GetPlatformReviewsAsync();
        Task<object> GetPlatformReviewsWithStatsAsync();
    }
}
