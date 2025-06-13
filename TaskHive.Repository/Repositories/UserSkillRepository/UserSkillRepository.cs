using Microsoft.EntityFrameworkCore;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.UserSkillRepository
{
    public class UserSkillRepository : IUserSkillRepository
    {
        private readonly AppDbContext _context;

        public UserSkillRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserSkill userSkill)
        {
            await _context.UserSkills.AddAsync(userSkill);
        }

        public async Task<IEnumerable<UserSkill>> GetByUserIdAsync(int userId)
        {
            return await _context.UserSkills
                .Include(us => us.Category)
                .Where(us => us.UserId == userId)
                .ToListAsync();
        }

        public async Task DeleteByUserIdAsync(int userId)
        {
            var userSkills = await _context.UserSkills
                .Where(us => us.UserId == userId)
                .ToListAsync();

            _context.UserSkills.RemoveRange(userSkills);
        }

        public async Task<bool> ExistsAsync(int userId, int categoryId)
        {
            return await _context.UserSkills
                .AnyAsync(us => us.UserId == userId && us.CategoryId == categoryId);
        }
    }
}