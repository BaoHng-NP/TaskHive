using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Repositories.UserRepository;
using TaskHive.Repository.Repositories.UserSkillRepository;
using TaskHive.Repository.Repositories.ApplicationRepository;
using TaskHive.Repository.Repositories.CategoryRepository;
using TaskHive.Repository.Repositories.JobPostRepository;
using TaskHive.Repository.Repositories;
using TaskHive.Repository.Repositories.EmailVerificationRepository;
using TaskHive.Repository.Repositories.MembershipRepository;
using TaskHive.Repository.Repositories.UserMembershipRepository;
using TaskHive.Repository.Repositories.PaymentRepository;

namespace TaskHive.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IUserRepository Users { get; }
        public IUserSkillRepository UserSkills { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IEmailVerificationRepository EmailVerifications { get; }
        public IApplicationRepository Applications { get; }
        public ICategoryRepository Categories { get; }
        public IJobPostRepository JobPosts { get; }
        public IMembershipRepository Memberships { get; }
        public IUserMembershipRepository UserMemberships { get; }
        public IPaymentRepository Payments { get; }

        public UnitOfWork(AppDbContext context, 
            IUserRepository userRepository, 
            IApplicationRepository applicationRepository, 
            IUserSkillRepository userSkillRepository, 
            IEmailVerificationRepository emailVerificationRepository, 
            IRefreshTokenRepository refreshTokenRepository, 
            ICategoryRepository categoryRepository,
            IJobPostRepository jobPostRepository,
            IMembershipRepository membershipRepository,
            IUserMembershipRepository userMembershipRepository,
            IPaymentRepository paymentRepository)
        {
            _context = context;
            Users = userRepository;
            UserSkills = userSkillRepository;
            EmailVerifications = emailVerificationRepository;
            RefreshTokens = refreshTokenRepository;
            Applications = applicationRepository;
            Categories = categoryRepository;
            JobPosts = jobPostRepository;
            Memberships = membershipRepository;
            UserMemberships = userMembershipRepository;
            Payments = paymentRepository;
        }


        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
