using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
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

        private const string InitialStatus = "Pending";
        private const long MaxCvBytes = 10 * 1024 * 1024;
        private static readonly string[] AllowedCvExtensions = { ".pdf", ".doc", ".docx" };

        private static string? ValidateCv(IFormFile? file)
        {
            if (file == null) return null;
            if (file.Length == 0 || file.Length > MaxCvBytes) return "CV file must be between 1 byte and 10MB";
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return AllowedCvExtensions.Contains(ext) ? null : "CV must be a PDF, DOC or DOCX file";
        }

        private async Task<string?> UploadCvAsync(IFormFile file, int freelancerId)
        {
            await using var stream = file.OpenReadStream();
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription($"cv{ext}", stream),
                // Không dùng tên file gốc của người dùng làm PublicId
                PublicId = $"cvs/{freelancerId}_{Guid.NewGuid():N}{ext}"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.Error == null ? uploadResult.SecureUrl?.ToString() : null;
        }

        public async Task<(ApplicationResponseDto? application, string? error)> AddApplicationAsync(AddApplicationRequestDto dto, int freelancerId)
        {
            var cvError = ValidateCv(dto.CVFile);
            if (cvError != null) return (null, cvError);

            var jobPost = await _unitOfWork.JobPosts.GetJobPostByIdAsync(dto.JobPostId);
            if (jobPost == null) return (null, "Job post not found");

            var freelancer = await _unitOfWork.Users.GetFreelancerByIdAsync(freelancerId);
            if (freelancer == null) return (null, "Only freelancers can apply");

            if (await _unitOfWork.Applications.GetByJobPostAndFreelancerAsync(dto.JobPostId, freelancerId) != null)
                return (null, "You have already applied to this job");

            if (freelancer.RemainingSlots <= 0)
                return (null, "Not enough slots. Please buy more slots or a membership.");

            var application = _mapper.Map<Application>(dto);
            application.FreelancerId = freelancerId;   // luôn lấy từ token, bỏ qua giá trị client gửi
            application.Status = InitialStatus;        // freelancer không được tự đặt trạng thái
            application.AppliedAt = DateTime.UtcNow;
            application.UpdatedAt = DateTime.UtcNow;
            application.IsDeleted = false;

            if (dto.CVFile != null)
            {
                var url = await UploadCvAsync(dto.CVFile, freelancerId);
                if (url == null) return (null, "CV upload failed");
                application.CVFile = url;
            }

            // Trừ slot trên server; được lưu cùng một SaveChanges với application.
            freelancer.RemainingSlots -= 1;
            freelancer.UpdatedAt = DateTime.UtcNow;

            var success = await _unitOfWork.Applications.AddApplicationAsync(application);
            if (!success)
                return (null, "Thêm application thất bại");

            var result = await _unitOfWork.Applications.GetApplicationByIdAsync(application.ApplicationId);
            return (_mapper.Map<ApplicationResponseDto>(result), null);
        }

        public async Task<(ApplicationResponseDto? application, string? error)> UpdateApplicationAsync(
            UpdateApplicationRequestDto dto, bool canEditContent, bool canEditStatus)
        {
            var existing = await _unitOfWork.Applications.GetApplicationByIdAsync(dto.ApplicationId);
            if (existing == null)
                return (null, "Application không tồn tại");

            if (canEditContent)
            {
                var cvError = ValidateCv(dto.CVFile);
                if (cvError != null) return (null, cvError);

                if (dto.CVFile != null)
                {
                    var url = await UploadCvAsync(dto.CVFile, existing.FreelancerId);
                    if (url == null) return (null, "CV upload failed");
                    existing.CVFile = url;
                }
                if (dto.CoverLetter != null) existing.CoverLetter = dto.CoverLetter;
                if (dto.BidAmount != null) existing.BidAmount = dto.BidAmount;
            }

            if (canEditStatus && !string.IsNullOrWhiteSpace(dto.Status))
                existing.Status = dto.Status;

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

        public async Task<(int FreelancerId, int EmployerId)?> GetParticipantsAsync(int applicationId)
        {
            var application = await _unitOfWork.Applications.GetApplicationByIdAsync(applicationId);
            if (application == null || application.IsDeleted) return null;
            return (application.FreelancerId, application.JobPost.EmployerId);
        }
    }
}