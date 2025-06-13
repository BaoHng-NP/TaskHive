// TaskHive.Service/Services/JobPostService/JobPostService.cs
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.Repositories.JobPostRepository;
using TaskHive.Service.DTOs.Requests.JobPost;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.JobPostService
{
    public class JobPostService : IJobPostService
    {
        private readonly IJobPostRepository _jobPostRepository;
        private readonly IMapper _mapper;

        public JobPostService(IJobPostRepository jobPostRepository, IMapper mapper)
        {
            _jobPostRepository = jobPostRepository;
            _mapper = mapper;
        }

        public async Task<JobPostResponseDto?> GetJobPostByIdAsync(int jobPostId)
        {
            var jobPost = await _jobPostRepository.GetJobPostByIdAsync(jobPostId);
            return jobPost == null ? null : _mapper.Map<JobPostResponseDto>(jobPost);
        }

        public async Task<List<JobPostResponseDto>> GetJobPostsByEmployerIdAsync(int employerId)
        {
            var jobPosts = await _jobPostRepository.GetJobPostsByEmployerIdAsync(employerId);
            return _mapper.Map<List<JobPostResponseDto>>(jobPosts);
        }

        public async Task<List<JobPostResponseDto>> GetAllJobPostsAsync()
        {
            var jobPosts = await _jobPostRepository.GetAllJobPostsAsync();
            return _mapper.Map<List<JobPostResponseDto>>(jobPosts);
        }

        public async Task<List<JobPostResponseDto>> GetJobPostsByCategoryIdAsync(int categoryId)
        {
            var jobPosts = await _jobPostRepository.GetJobPostsByCategoryIdAsync(categoryId);
            return _mapper.Map<List<JobPostResponseDto>>(jobPosts);
        }

        public async Task<(JobPostResponseDto? jobPost, string? error)> AddJobPostAsync(AddJobPostRequestDto jobPostDto)
        {
            var jobPost = _mapper.Map<JobPost>(jobPostDto);
            jobPost.CreatedAt = DateTime.UtcNow;
            jobPost.UpdatedAt = DateTime.UtcNow;
            jobPost.IsDeleted = false;

            var success = await _jobPostRepository.AddJobPostAsync(jobPost);
            if (!success) return (null, "Failed to add job post");

            var result = await _jobPostRepository.GetJobPostByIdAsync(jobPost.JobPostId);
            return (_mapper.Map<JobPostResponseDto>(result), null);
        }

        public async Task<(JobPostResponseDto? jobPost, string? error)> UpdateJobPostAsync(UpdateJobPostRequestDto jobPostDto)
        {
            var existingJobPost = await _jobPostRepository.GetJobPostByIdAsync(jobPostDto.JobPostId);
            if (existingJobPost == null) return (null, "Job post not found");

            _mapper.Map(jobPostDto, existingJobPost);
            existingJobPost.UpdatedAt = DateTime.UtcNow;

            var success = await _jobPostRepository.UpdateJobPostAsync(existingJobPost);
            if (!success) return (null, "Failed to update job post");

            var result = await _jobPostRepository.GetJobPostByIdAsync(existingJobPost.JobPostId);
            return (_mapper.Map<JobPostResponseDto>(result), null);
        }

        public async Task<string?> DeleteJobPostAsync(int jobPostId)
        {
            var jobPost = await _jobPostRepository.GetJobPostByIdAsync(jobPostId);
            if (jobPost == null) return "Job post not found";

            var success = await _jobPostRepository.DeleteJobPostAsync(jobPostId);
            return success ? null : "Failed to delete job post";
        }
    }
}