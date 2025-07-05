using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.UserMembershipRepository
{
    public interface IUserMembershipRepository
    {
        Task<bool> AddUserMembershipAsync(UserMembership userMembership);
        Task<bool> UpdateUserMembershipAsync(UserMembership userMembership);
        Task<bool> DeleteUserMembershipAsync(int userMembershipId);
        Task<UserMembership?> GetUserMembershipByIdAsync(int userMembershipId);
        Task<List<UserMembership>> GetAllUserMembershipsAsync();
        Task<List<UserMembership>> FindAsync(Expression<Func<UserMembership, bool>> predicate);

    }
}
