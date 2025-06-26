using Microsoft.EntityFrameworkCore;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.ReviewRepository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        // ✅ FIX: Thêm access modifier
        public ReviewRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(re => re.Reviewee)
                .Include(re => re.Reviewer)
                .Where(re => !re.IsDeleted)
                .OrderByDescending(re => re.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Review>> GetByPostIdAsync(int id)
        {
            return await _context.Reviews
                .Include(re => re.Reviewee)
                .Include(re => re.Reviewer)
                .Where(re => re.JobPostId == id && !re.IsDeleted)
                .OrderByDescending(re => re.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(re => re.Reviewee)
                .Include(re => re.Reviewer)
                .FirstOrDefaultAsync(re => re.ReviewId == id && !re.IsDeleted);
        }

        public async Task CreateAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null || review.IsDeleted)
                return false;

            review.IsDeleted = true;
            _context.Reviews.Update(review);
            return true;
        }

        public async Task<List<Review>> GetByReviewerIdAsync(int reviewerId)
        {
            return await _context.Reviews
                .Include(re => re.Reviewee)
                .Include(re => re.Reviewer)
                .Where(re => re.ReviewerId == reviewerId && !re.IsDeleted)
                .OrderByDescending(re => re.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Review>> GetByRevieweeIdAsync(int revieweeId)
        {
            return await _context.Reviews
                .Include(re => re.Reviewee)
                .Include(re => re.Reviewer)
                .Where(re => re.RevieweeId == revieweeId && !re.IsDeleted)
                .OrderByDescending(re => re.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int reviewerId, int revieweeId, int jobPostId)
        {
            return await _context.Reviews
                .AnyAsync(re => re.ReviewerId == reviewerId &&
                              re.RevieweeId == revieweeId &&
                              re.JobPostId == jobPostId &&
                              !re.IsDeleted);
        }

        public async Task<List<Review>> GetByPostIdAndReviewerIdAsync(int jobPostId, int reviewerId)
        {
            return await _context.Reviews
                .Include(re => re.Reviewee)
                .Include(re => re.Reviewer)
                .Where(re => re.JobPostId == jobPostId && re.ReviewerId == reviewerId && !re.IsDeleted)
                .OrderByDescending(re => re.CreatedAt)
                .ToListAsync();
        }

        public Task<List<Review>> GetByPostIdAndRevieweeIdAsync(int jobPostId, int revieweeId)
        {
            return _context.Reviews
                .Include(re => re.Reviewee)
                .Include(re => re.Reviewer)
                .Where(re => re.JobPostId == jobPostId && re.RevieweeId == revieweeId && !re.IsDeleted)
                .OrderByDescending(re => re.CreatedAt)
                .ToListAsync();
        }
    }
}
