using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Repositories.JobPostRepository;
using TaskHive.Service.DTOs.Requests.JobPost;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.JobPostService
{
    public interface IJobPostService
    {
        Task<JobPostResponseDto?> GetJobPostByIdAsync(int jobPostId);
        Task<List<JobPostResponseDto>> GetJobPostsByEmployerIdAsync(int employerId);
        Task<List<JobPostResponseDto>> GetAllJobPostsAsync();
        Task<List<JobPostResponseDto>> GetJobPostsByCategoryIdAsync(int categoryId);
        Task<(JobPostResponseDto? jobPost, string? error)> AddJobPostAsync(AddJobPostRequestDto jobPostDto);
        Task<(JobPostResponseDto? jobPost, string? error)> UpdateJobPostAsync(UpdateJobPostRequestDto jobPostDto);
        Task<string?> DeleteJobPostAsync(int jobPostId);

        Task<PagedResult<JobPostResponseDto>> GetJobPostsPagedAsync(JobQueryParam parameters);
    }
}