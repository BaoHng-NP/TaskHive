using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
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

        public async Task<(ApplicationResponseDto? application, string? error)> AddApplicationAsync(AddApplicationRequestDto dto)
        {
            var application = _mapper.Map<Application>(dto);

            // 1. Upload CV nếu client gửi lên
            if (dto.CVFile != null)
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(dto.CVFile.FileName, dto.CVFile.OpenReadStream()),
                    PublicId = $"cvs/{Path.GetFileNameWithoutExtension(dto.CVFile.FileName)}_{Guid.NewGuid()}"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                application.CVFile = uploadResult.SecureUrl.ToString();
            }

            // 2. Các trường mặc định
            application.AppliedAt = DateTime.UtcNow;
            application.IsDeleted = false;

            // 3. Lưu
            var success = await _unitOfWork.Applications.AddApplicationAsync(application);
            if (!success)
                return (null, "Thêm application thất bại");

            var result = await _unitOfWork.Applications.GetApplicationByIdAsync(application.ApplicationId);
            return (_mapper.Map<ApplicationResponseDto>(result), null);
        }

        public async Task<(ApplicationResponseDto? application, string? error)> UpdateApplicationAsync(UpdateApplicationRequestDto dto)
        {
            var existing = await _unitOfWork.Applications.GetApplicationByIdAsync(dto.ApplicationId);
            if (existing == null)
                return (null, "Application không tồn tại");

            // Upload CV mới nếu client gửi
            if (dto.CVFile != null)
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(dto.CVFile.FileName, dto.CVFile.OpenReadStream()),
                    PublicId = $"cvs/{Path.GetFileNameWithoutExtension(dto.CVFile.FileName)}_{Guid.NewGuid()}"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                existing.CVFile = uploadResult.SecureUrl.ToString();
            }

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _unitOfWork.Applications.UpdateApplicationAsync(existing);
            if (!updated)
                return (null, "Cập nhật thất bại");

            var result = await _unitOfWork.Applications.GetApplicationByIdAsync(existing.ApplicationId);
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