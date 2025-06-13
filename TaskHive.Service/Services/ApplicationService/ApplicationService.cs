using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.Repositories.ApplicationRepository;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.Application;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.ApplicationService
{
    public class ApplicationService : IApplicationService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApplicationResponseDto?> GetApplicationByIdAsync(int applicationId)
        {
            var application = await _unitOfWork.Applications.GetApplicationByIdAsync(applicationId);
            return application == null ? null : _mapper.Map<ApplicationResponseDto>(application);
        }

        public async Task<List<ApplicationResponseDto>> GetApplicationsByJobPostIdAsync(int jobPostId)
        {
            var applications = await _unitOfWork.Applications.GetApplicationsByJobPostIdAsync(jobPostId);
            return _mapper.Map<List<ApplicationResponseDto>>(applications);
        }

        public async Task<List<ApplicationResponseDto>> GetApplicationsByFreelancerIdAsync(int freelancerId)
        {
            var applications = await _unitOfWork.Applications.GetApplicationsByFreelancerIdAsync(freelancerId);
            return _mapper.Map<List<ApplicationResponseDto>>(applications);
        }

        public async Task<(ApplicationResponseDto? application, string? error)> AddApplicationAsync(AddApplicationRequestDto applicationDto)
        {
            var application = _mapper.Map<Application>(applicationDto);
            application.AppliedAt = DateTime.UtcNow;
            application.IsDeleted = false;

            var success = await _unitOfWork.Applications.AddApplicationAsync(application);
            if (!success) return (null, "Failed to add application");

            var result = await _unitOfWork.Applications.GetApplicationByIdAsync(application.ApplicationId);
            return (_mapper.Map<ApplicationResponseDto>(result), null);
        }

        public async Task<(ApplicationResponseDto? application, string? error)> UpdateApplicationAsync(UpdateApplicationRequestDto applicationDto)
        {
            var existingApplication = await _unitOfWork.Applications.GetApplicationByIdAsync(applicationDto.ApplicationId);
            if (existingApplication == null) return (null, "Application not found");

            _mapper.Map(applicationDto, existingApplication);
            existingApplication.UpdatedAt = DateTime.UtcNow;

            var success = await _unitOfWork.Applications.UpdateApplicationAsync(existingApplication);
            if (!success) return (null, "Failed to update application");

            var result = await _unitOfWork.Applications.GetApplicationByIdAsync(existingApplication.ApplicationId);
            return (_mapper.Map<ApplicationResponseDto>(result), null);
        }

        public async Task<string?> DeleteApplicationAsync(int applicationId)
        {
            var application = await _unitOfWork.Applications.GetApplicationByIdAsync(applicationId);
            if (application == null) return "Application not found";

            var success = await _unitOfWork.Applications.DeleteApplicationAsync(applicationId);
            return success ? null : "Failed to delete application";
        }
    }
}