using TaskHive.Repository.Repositories;
using TaskHive.Repository.Repositories.ApplicationRepository;
using TaskHive.Repository.Repositories.CategoryRepository;
using TaskHive.Repository.Repositories.EmailVerificationRepository;
using TaskHive.Repository.Repositories.GenericRepository;
using TaskHive.Repository.Repositories.JobPostRepository;
using TaskHive.Repository.Repositories.MembershipRepository;
using TaskHive.Repository.Repositories.PaymentRepository;
using TaskHive.Repository.Repositories.SlotPurchaseRepository;
using TaskHive.Repository.Repositories.UserMembershipRepository;
using TaskHive.Repository.Repositories.UserRepository;
using TaskHive.Repository.Repositories.UserSkillRepository;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.Mappings;
using TaskHive.Service.Services.ApplicationService;
using TaskHive.Service.Services.CategoryService;
using TaskHive.Service.Services.CloudinaryService;
using TaskHive.Service.Services.EmailService;
using TaskHive.Service.Services.JobPostService;
using TaskHive.Service.Services.MembershipService;
using TaskHive.Service.Services.PaymentService;
using TaskHive.Service.Services.SlotPurchaseService;
using TaskHive.Service.Services.UserMembershipService;
using TaskHive.Service.Services.UserService;

namespace TaskHive.API
{
    public static class DepencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Add AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // Các service khác
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJobPostService, JobPostService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IUserMembershipService, UserMembershipService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ISlotPurchaseService, SlotPurchaseService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            //Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserSkillRepository, UserSkillRepository>();
            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
            services.AddScoped<IUserSkillRepository, UserSkillRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IJobPostRepository, JobPostRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IMembershipRepository, MembershipRepository>();
            services.AddScoped<IUserMembershipRepository, UserMembershipRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ISlotPurchaseRepository, SlotPurchaseRepository>();

            return services;

        }
    }
}
