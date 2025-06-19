using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.MembershipRepository
{
    public interface IMembershipRepository
    {
        Task<bool> AddMembershipAsync(Membership membership);
        Task<bool> UpdateMembershipAsync(Membership membership);
        Task<bool> DeleteMembershipAsync(int membershipId);
        Task<Membership?> GetMembershipByIdAsync(int membershipId);
        Task<List<Membership>> GetAllMembershipsAsync();
    }
}
