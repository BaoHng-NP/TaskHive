using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs.Requests.Membership;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.MembershipService
{
    public interface IMembershipService
    {
        Task<MembershipResponseDto?> GetMembershipByIdAsync(int membershipId);
        Task<List<MembershipResponseDto>> GetAllMembershipsAsync();
        Task<(MembershipResponseDto? membership, string? error)> AddMembershipAsync(AddMembershipRequestDto membershipDto);
        Task<(MembershipResponseDto? membership, string? error)> UpdateMembershipAsync(UpdateMembershipRequestDto membershipDto);
        Task<string?> DeleteMembershipAsync(int membershipId);
    }
}
