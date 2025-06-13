using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.ApplicationRepository
{
    public interface IApplicationRepository
    {
        Task<Application?> GetApplicationByIdAsync(int applicationId);
        Task<List<Application>> GetApplicationsByJobPostIdAsync(int jobPostId);
        Task<List<Application>> GetApplicationsByFreelancerIdAsync(int freelancerId);
        Task<bool> AddApplicationAsync(Application application);
        Task<bool> UpdateApplicationAsync(Application application);
        Task<bool> DeleteApplicationAsync(int applicationId);
    }
}