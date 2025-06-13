using TaskHive.Repository.Repositories.EmailVerificationRepository;
using TaskHive.Repository.Repositories.GenericRepository;
using TaskHive.Repository.Repositories.UserRepository;
using TaskHive.Repository.Repositories.UserSkillRepository;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.Services.EmailService;
using TaskHive.Service.Services.UserService;

namespace TaskHive.API
{
    public static class DepencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();


            //Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserSkillRepository, UserSkillRepository>();
            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
            services.AddScoped<IUserSkillRepository, UserSkillRepository>();






            return services;

        }
    }
}
