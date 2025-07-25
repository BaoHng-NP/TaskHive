using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.Repositories.BlogPostRepository;

namespace TaskHive.Repository.Repositories.ReviewRepository
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllAsync();
        Task<List<Review>> GetByPostIdAsync(int id);
        Task<Review?> GetByIdAsync(int id);
        Task CreateAsync(Review review);
        Task UpdateAsync(Review review);
        Task<bool> DeleteAsync(int id);

        Task<List<Review>> GetByReviewerIdAsync(int reviewerId);
        Task<List<Review>> GetByRevieweeIdAsync(int revieweeId);
        Task<bool> ExistsAsync(int reviewerId, int revieweeId, int jobPostId);

        Task<List<Review>> GetByPostIdAndReviewerIdAsync(int jobPostId, int reviewerId);
        Task<List<Review>> GetByPostIdAndRevieweeIdAsync(int jobPostId, int revieweeId);
        // Platform reviews
        Task<Review?> GetPlatformReviewByUserAsync(int reviewerId, int platformRevieweeId);
        Task<List<Review>> GetPlatformReviewsAsync(int platformRevieweeId);
    }
}
