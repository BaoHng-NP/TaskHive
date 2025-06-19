using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
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
    }
}
