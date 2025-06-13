using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Repositories.EmailVerificationRepository;
using TaskHive.Repository.Repositories.UserRepository;
using TaskHive.Repository.Repositories.UserSkillRepository;

namespace TaskHive.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IUserRepository Users { get; }
        public IUserSkillRepository UserSkills { get; }

        public IEmailVerificationRepository EmailVerifications { get; }

        public UnitOfWork(AppDbContext context, IUserRepository userRepository, IUserSkillRepository userSkillRepository, IEmailVerificationRepository emailVerificationRepository)
        {
            _context = context;
            Users = userRepository;
            UserSkills = userSkillRepository;
            EmailVerifications = emailVerificationRepository;
        }


        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
