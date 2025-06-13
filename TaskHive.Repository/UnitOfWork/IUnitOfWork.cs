using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Repositories.UserRepository;
using TaskHive.Repository.Repositories.UserSkillRepository;
using TaskHive.Repository.Repositories.EmailVerificationRepository;

namespace TaskHive.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IUserSkillRepository UserSkills { get; }
        IEmailVerificationRepository EmailVerifications { get; }
        
        Task<int> SaveChangesAsync();
    }
}
