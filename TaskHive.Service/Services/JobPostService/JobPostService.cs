using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.Repositories.JobPostRepository;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.JobPost;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.JobPostService
{
    public class JobPostService : IJobPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JobPostService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ✅ Add new method with Projection
        public async Task<PagedResult<JobPostWithRatingResponseDto>> GetJobPostsWithRatingsPagedAsync(JobQueryParam param)
        {
            try
            {
                var query = _unitOfWork.JobPosts.GetJobPostsQueryable();

                // ✅ Apply search filter
                if (!string.IsNullOrEmpty(param.Search))
                {
                    var searchLower = param.Search.ToLower();
                    query = query.Where(j => j.Title.ToLower().Contains(searchLower));
                }

                // ✅ Apply category filter
                if (param.CategoryIds != null && param.CategoryIds.Count > 0)
                {
                    query = query.Where(j => param.CategoryIds.Contains(j.CategoryId));
                }

                // ✅ Get total count
                int totalCount = await query.CountAsync();

                // ✅ Projection to DTO - Single optimized query
                var jobPostDtos = await query
                    .OrderByDescending(j => j.CreatedAt)
                    .Skip((param.Page - 1) * param.PageSize)
                    .Take(param.PageSize)
                    .Select(j => new JobPostWithRatingResponseDto
                    {
                        JobPostId = j.JobPostId,
                        EmployerId = j.EmployerId,
                        EmployerName = j.Employer.FullName,
                        Title = j.Title,
                        Description = j.Description,
                        CategoryId = j.CategoryId,
                        CategoryName = j.Category.Name,
                        Location = j.Location,
                        SalaryMin = j.SalaryMin,
                        SalaryMax = j.SalaryMax,
                        JobType = j.JobType,
                        Status = j.Status,
                        CreatedAt = j.CreatedAt,
                        UpdatedAt = j.UpdatedAt,
                        Deadline = j.Deadline,
                        // ✅ Efficient aggregation in SQL
                        ReviewCount = j.Employer.ReviewsReceived.Count(),
                        AverageRating = j.Employer.ReviewsReceived.Any()
                            ? j.Employer.ReviewsReceived.Average(r => (decimal)r.Rating)
                            : 0m
                    })
                    .ToListAsync();

                return new PagedResult<JobPostWithRatingResponseDto>
                {
                    Items = jobPostDtos,
                    TotalItems = totalCount,
                    Page = param.Page,
                    PageSize = param.PageSize
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching paged job posts with ratings.", ex);
            }
        }

        // ✅ Keep all existing methods unchanged
        public async Task<JobPostResponseDto?> GetJobPostByIdAsync(int jobPostId)
        {
            var jobPost = await _unitOfWork.JobPosts.GetJobPostByIdAsync(jobPostId);
            return jobPost == null ? null : _mapper.Map<JobPostResponseDto>(jobPost);
        }

        public async Task<List<JobPostResponseDto>> GetJobPostsByEmployerIdAsync(int employerId)
        {
            var jobPosts = await _unitOfWork.JobPosts.GetJobPostsByEmployerIdAsync(employerId);
            return _mapper.Map<List<JobPostResponseDto>>(jobPosts);
        }

        public async Task<List<JobPostResponseDto>> GetAllJobPostsAsync()
        {
            var jobPosts = await _unitOfWork.JobPosts.GetAllJobPostsAsync();
            return _mapper.Map<List<JobPostResponseDto>>(jobPosts);
        }

        public async Task<List<JobPostResponseDto>> GetJobPostsByCategoryIdAsync(int categoryId)
        {
            var jobPosts = await _unitOfWork.JobPosts.GetJobPostsByCategoryIdAsync(categoryId);
            return _mapper.Map<List<JobPostResponseDto>>(jobPosts);
        }

        public async Task<(JobPostResponseDto? jobPost, string? error)> AddJobPostAsync(AddJobPostRequestDto jobPostDto)
        {
            var jobPost = _mapper.Map<JobPost>(jobPostDto);
            jobPost.CreatedAt = DateTime.UtcNow;
            jobPost.UpdatedAt = DateTime.UtcNow;
            jobPost.IsDeleted = false;

            var success = await _unitOfWork.JobPosts.AddJobPostAsync(jobPost);
            if (!success) return (null, "Failed to add job post");

            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.JobPosts.GetJobPostByIdAsync(jobPost.JobPostId);
            return (_mapper.Map<JobPostResponseDto>(result), null);
        }

        public async Task<(JobPostResponseDto? jobPost, string? error)> UpdateJobPostAsync(UpdateJobPostRequestDto jobPostDto)
        {
            var existingJobPost = await _unitOfWork.JobPosts.GetJobPostByIdAsync(jobPostDto.JobPostId);
            if (existingJobPost == null) return (null, "Job post not found");

            _mapper.Map(jobPostDto, existingJobPost);
            existingJobPost.UpdatedAt = DateTime.UtcNow;

            var success = await _unitOfWork.JobPosts.UpdateJobPostAsync(existingJobPost);
            if (!success) return (null, "Failed to update job post");

            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.JobPosts.GetJobPostByIdAsync(existingJobPost.JobPostId);
            return (_mapper.Map<JobPostResponseDto>(result), null);
        }

        public async Task<string?> DeleteJobPostAsync(int jobPostId)
        {
            var jobPost = await _unitOfWork.JobPosts.GetJobPostByIdAsync(jobPostId);
            if (jobPost == null) return "Job post not found";

            var success = await _unitOfWork.JobPosts.DeleteJobPostAsync(jobPostId);
            if (!success) return "Failed to delete job post";

            await _unitOfWork.SaveChangesAsync();
            return null;
        }

        public async Task<PagedResult<JobPostResponseDto>> GetJobPostsPagedAsync(JobQueryParam param)
        {
            try
            {
                var (jobPosts, totalCount) = await _unitOfWork.JobPosts.GetJobPostsPagedAsync(param);

                var jobPostDtos = _mapper.Map<List<JobPostResponseDto>>(jobPosts);

                return new PagedResult<JobPostResponseDto>
                {
                    Items = jobPostDtos,
                    TotalItems = totalCount,
                    Page = param.Page,
                    PageSize = param.PageSize
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching paged job posts.", ex);
            }
        }
    }
}
