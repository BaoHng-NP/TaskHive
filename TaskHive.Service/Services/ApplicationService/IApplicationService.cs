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
        Task<(ApplicationResponseDto? application, string? error)> AddApplicationAsync(AddApplicationRequestDto applicationDto);
        Task<(ApplicationResponseDto? application, string? error)> UpdateApplicationAsync(UpdateApplicationRequestDto applicationDto);
        Task<string?> DeleteApplicationAsync(int applicationId);
    }
}