using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task AddAsync(User user) =>
            await _context.Users.AddAsync(user);

        public async Task<bool> ExistsByEmailAsync(string email) =>
            await _context.Users.AnyAsync(u => u.Email == email);

        public async Task<User?> GetByGoogleIdAsync(string googleId) =>
            await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

        public async Task<User?> GetByIdAsync(int userId) =>
            await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
        }

        public async Task AddFreelancerAsync(Freelancer freelancer) =>
            await _context.Set<Freelancer>().AddAsync(freelancer);

        public async Task AddClientAsync(Client client) =>
            await _context.Set<Client>().AddAsync(client);

        public async Task<Freelancer?> GetFreelancerByIdAsync(int userId) =>
            await _context.Set<Freelancer>()
                .Include(f => f.UserSkills)
                    .ThenInclude(us => us.Category)
                .FirstOrDefaultAsync(f => f.UserId == userId);

        public async Task<Client?> GetClientByIdAsync(int userId) =>
            await _context.Set<Client>().FirstOrDefaultAsync(c => c.UserId == userId);

        public async Task<List<User>> GetAllUsersAsync() =>
            await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        public IQueryable<Freelancer> GetFreelancersQueryable()
        {
            // Include skills để project ra tên category
            return _context.Set<Freelancer>()
                .Include(f => f.UserSkills)
                    .ThenInclude(us => us.Category);
        }
    }

}
