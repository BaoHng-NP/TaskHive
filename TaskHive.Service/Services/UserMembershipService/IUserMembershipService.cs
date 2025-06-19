using TaskHive.Service.DTOs.Requests.UserMembership;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.UserMembershipService
{
    public interface IUserMembershipService
    {
        Task<UserMembershipResponseDto?> GetUserMembershipByIdAsync(int id);
        Task<List<UserMembershipResponseDto>> GetAllUserMembershipsAsync();
        Task<(UserMembershipResponseDto?, string?)> AddUserMembershipAsync(AddUserMembershipRequestDto dto);
        Task<(UserMembershipResponseDto?, string?)> UpdateUserMembershipAsync(UpdateUserMembershipRequestDto dto);
        Task<string?> DeleteUserMembershipAsync(int id);
    }
}
