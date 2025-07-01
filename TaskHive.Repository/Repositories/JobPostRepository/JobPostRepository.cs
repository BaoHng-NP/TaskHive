using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.JobPostRepository
{
    public class JobPostRepository : IJobPostRepository
    {
        private readonly AppDbContext _context;

        public JobPostRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Add new method for Projection
        public IQueryable<JobPost> GetJobPostsQueryable()
        {
            return _context.JobPosts.Where(j => !j.IsDeleted);
        }

        // ✅ Keep all existing methods unchanged
        public async Task<JobPost?> GetJobPostByIdAsync(int jobPostId)
        {
            return await _context.JobPosts
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.JobPostId == jobPostId && !j.IsDeleted);
        }

        public async Task<List<JobPost>> GetJobPostsByEmployerIdAsync(int employerId)
        {
            return await _context.JobPosts
                .Include(j => j.Category)
                .Where(j => j.EmployerId == employerId && !j.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<JobPost>> GetAllJobPostsAsync()
        {
            return await _context.JobPosts
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .Where(j => !j.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<JobPost>> GetJobPostsByCategoryIdAsync(int categoryId)
        {
            return await _context.JobPosts
                .Include(j => j.Employer)
                .Where(j => j.CategoryId == categoryId && !j.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> AddJobPostAsync(JobPost jobPost)
        {
            await _context.JobPosts.AddAsync(jobPost);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateJobPostAsync(JobPost jobPost)
        {
            _context.JobPosts.Update(jobPost);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteJobPostAsync(int jobPostId)
        {
            var jobPost = await _context.JobPosts.FindAsync(jobPostId);
            if (jobPost == null) return false;

            jobPost.IsDeleted = true;
            _context.JobPosts.Update(jobPost);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<(List<JobPost>, int TotalCount)> GetJobPostsPagedAsync(JobQueryParam param)
        {
            var jobList = _context.JobPosts
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .Where(j => !j.IsDeleted);

            if (!string.IsNullOrEmpty(param.Search))
            {
                var searchLower = param.Search.ToLower();
                jobList = jobList.Where(j => j.Title.ToLower().Contains(searchLower));
            }

            if (param.CategoryIds != null && param.CategoryIds.Count > 0)
            {
                jobList = jobList.Where(j => param.CategoryIds.Contains(j.CategoryId));
            }

            int totalCount = await jobList.CountAsync();

            var jobs = await jobList
                .OrderByDescending(bp => bp.CreatedAt)
                .Skip((param.Page - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            return (jobs, totalCount);
        }
    }
}