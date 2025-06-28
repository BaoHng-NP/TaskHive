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

        public async Task<(List<JobPost>,int TotalCount)> GetJobPostsPagedAsync(JobQueryParam pram)
        {
            var jobList = _context.JobPosts
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .Where(j => !j.IsDeleted);

            if (!string.IsNullOrEmpty(pram.Search))
            {
                var searchLower = pram.Search.ToLower();
                jobList = jobList.Where(j => j.Title.ToLower().Contains(searchLower));
            }
            if (pram.CategoryIds != null && pram.CategoryIds.Count > 0)
            {
                jobList = jobList.Where(j => pram.CategoryIds.Contains(j.CategoryId));
            }
            int totalCount = await jobList.CountAsync();

            var jobs= await jobList
                .OrderByDescending(bp => bp.Title)
                .Skip((pram.Page - 1) * pram.PageSize)
                .Take(pram.PageSize)
                .ToListAsync();
            return (jobs, totalCount);
        }
    }
}