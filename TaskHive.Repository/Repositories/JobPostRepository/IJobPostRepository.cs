using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.JobPostRepository
{
    public interface IJobPostRepository
    {
        Task<JobPost?> GetJobPostByIdAsync(int jobPostId);
        Task<List<JobPost>> GetJobPostsByEmployerIdAsync(int employerId);
        Task<List<JobPost>> GetAllJobPostsAsync();
        Task<List<JobPost>> GetJobPostsByCategoryIdAsync(int categoryId);
        Task<bool> AddJobPostAsync(JobPost jobPost);
        Task<bool> UpdateJobPostAsync(JobPost jobPost);
        Task<bool> DeleteJobPostAsync(int jobPostId);
        Task<(List<JobPost>, int TotalCount)> GetJobPostsPagedAsync(JobQueryParam param);
        IQueryable<JobPost> GetJobPostsQueryable();
    }
}