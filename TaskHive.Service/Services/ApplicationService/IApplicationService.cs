using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs.Requests.Application;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.ApplicationService
{
    public interface IApplicationService
    {
        Task<ApplicationResponseDto?> GetApplicationByIdAsync(int applicationId);
        Task<List<ApplicationResponseDto>> GetApplicationsByJobPostIdAsync(int jobPostId);
        Task<List<ApplicationResponseDto>> GetApplicationsByFreelancerIdAsync(int freelancerId);
        /// <summary>Freelancer (lấy từ token) ứng tuyển; server kiểm tra trùng và trừ 1 slot.</summary>
        Task<(ApplicationResponseDto? application, string? error)> AddApplicationAsync(AddApplicationRequestDto applicationDto, int freelancerId);

        /// <summary>
        /// canEditContent: được sửa CoverLetter/BidAmount/CV (freelancer sở hữu).
        /// canEditStatus: được đổi Status (employer của job).
        /// </summary>
        Task<(ApplicationResponseDto? application, string? error)> UpdateApplicationAsync(
            UpdateApplicationRequestDto applicationDto, bool canEditContent, bool canEditStatus);

        Task<string?> DeleteApplicationAsync(int applicationId);

        /// <summary>Trả về (FreelancerId, EmployerId) của application để kiểm tra quyền; null nếu không tồn tại.</summary>
        Task<(int FreelancerId, int EmployerId)?> GetParticipantsAsync(int applicationId);
    }
}