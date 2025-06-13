using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.ApplicationRepository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;

        public ApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Application?> GetApplicationByIdAsync(int applicationId)
        {
            return await _context.Applications
                .Include(a => a.Freelancer)
                .Include(a => a.JobPost)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
        }

        public async Task<List<Application>> GetApplicationsByJobPostIdAsync(int jobPostId)
        {
            return await _context.Applications
                .Include(a => a.Freelancer)
                .Where(a => a.JobPostId == jobPostId && !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Application>> GetApplicationsByFreelancerIdAsync(int freelancerId)
        {
            return await _context.Applications
                .Include(a => a.JobPost)
                .Where(a => a.FreelancerId == freelancerId && !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> AddApplicationAsync(Application application)
        {
            await _context.Applications.AddAsync(application);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateApplicationAsync(Application application)
        {
            _context.Applications.Update(application);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteApplicationAsync(int applicationId)
        {
            var application = await _context.Applications.FindAsync(applicationId);
            if (application == null) return false;

            application.IsDeleted = true;
            _context.Applications.Update(application);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}