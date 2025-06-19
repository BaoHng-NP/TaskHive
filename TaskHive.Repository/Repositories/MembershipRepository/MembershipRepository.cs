using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.MembershipRepository
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly AppDbContext _context;
        public MembershipRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddMembershipAsync(Membership membership)
        {
            await _context.Memberships.AddAsync(membership);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateMembershipAsync(Membership membership)
        {
            _context.Memberships.Update(membership);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteMembershipAsync(int membershipId)
        {
            var membership = await _context.Memberships.FindAsync(membershipId);
            if (membership == null) return false;
            membership.IsDeleted = true;
            _context.Memberships.Update(membership);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Membership?> GetMembershipByIdAsync(int membershipId)
        {
            return await _context.Memberships
                .FirstOrDefaultAsync(m => m.MembershipId == membershipId && !m.IsDeleted);
        }
        public async Task<List<Membership>> GetAllMembershipsAsync()
        {
            return await _context.Memberships
                .Where(m => !m.IsDeleted)
                .ToListAsync();
        }
        public async Task<bool> AnyAsync(Expression<Func<Membership, bool>> predicate)
        {
            return await _context.Memberships.AnyAsync(predicate);
        }

    }
}
