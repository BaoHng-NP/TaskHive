using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<User?> GetByIdAsync(int userId);
        Task UpdateAsync(User user);

        Task AddFreelancerAsync(Freelancer freelancer);
        Task AddClientAsync(Client client);
        Task<Freelancer?> GetFreelancerByIdAsync(int userId);
        Task<Client?> GetClientByIdAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
    }

}
