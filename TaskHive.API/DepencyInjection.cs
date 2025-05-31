using TaskHive.Repository.Repositories.GenericRepository;
using TaskHive.Repository.Repositories.UserRepository;
using TaskHive.Repository.UnitOfWork;
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


            //Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            return services;

        }
    }
}
