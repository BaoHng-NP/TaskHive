using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Repositories;
using TaskHive.Repository.Repositories.ApplicationRepository;
using TaskHive.Repository.Repositories.BlogPostRepository;
using TaskHive.Repository.Repositories.CategoryRepository;
using TaskHive.Repository.Repositories.EmailVerificationRepository;
using TaskHive.Repository.Repositories.JobPostRepository;
using TaskHive.Repository.Repositories.MembershipRepository;
using TaskHive.Repository.Repositories.PaymentRepository;
using TaskHive.Repository.Repositories.ReviewRepository;
using TaskHive.Repository.Repositories.SlotPurchaseRepository;
using TaskHive.Repository.Repositories.UserMembershipRepository;
using TaskHive.Repository.Repositories.UserRepository;
using TaskHive.Repository.Repositories.UserSkillRepository;

namespace TaskHive.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IApplicationRepository Applications { get; }
        ICategoryRepository Categories { get; }
        IJobPostRepository JobPosts { get; }
        IUserSkillRepository UserSkills { get; }
        IEmailVerificationRepository EmailVerifications { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IMembershipRepository Memberships { get; }
        IUserMembershipRepository UserMemberships { get; }
        IPaymentRepository Payments { get; }
        ISlotPurchaseRepository SlotPurchases { get; }
        IBlogPostRepository BlogPosts { get; }

        IReviewRepository Reviews { get; }
        Task<int> SaveChangesAsync();
    }
}
