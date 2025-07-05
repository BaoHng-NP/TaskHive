using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.UserMembershipRepository
{
    public class UserMembershipRepository : IUserMembershipRepository
    {
        private readonly AppDbContext _context;

        public UserMembershipRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUserMembershipAsync(UserMembership userMembership)
        {
            await _context.UserMemberships.AddAsync(userMembership);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserMembershipAsync(UserMembership userMembership)
        {
            _context.UserMemberships.Update(userMembership);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserMembershipAsync(int userMembershipId)
        {
            var entity = await _context.UserMemberships.FindAsync(userMembershipId);
            if (entity == null) return false;
            _context.UserMemberships.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<UserMembership?> GetUserMembershipByIdAsync(int userMembershipId)
        {
            return await _context.UserMemberships
                .Include(x => x.User)
                .Include(x => x.Membership)
                .FirstOrDefaultAsync(x => x.UserMembershipId == userMembershipId);
        }

        public async Task<List<UserMembership>> GetAllUserMembershipsAsync()
        {
            return await _context.UserMemberships
                .Include(x => x.User)
                .Include(x => x.Membership)
                .ToListAsync();
        }
        public async Task<List<UserMembership>> FindAsync(Expression<Func<UserMembership, bool>> predicate)
        {
            return await _context.UserMemberships
                .Include(x => x.User)
                .Include(x => x.Membership)
                .Where(predicate)
                .ToListAsync();
        }
    }
}
