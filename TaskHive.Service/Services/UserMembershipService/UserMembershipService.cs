using AutoMapper;
using TaskHive.Repository.Entities;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.UserMembership;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.UserMembershipService
{
    public class UserMembershipService : IUserMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserMembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserMembershipResponseDto?> GetUserMembershipByIdAsync(int id)
        {
            var entity = await _unitOfWork.UserMemberships.GetUserMembershipByIdAsync(id);
            return entity == null ? null : _mapper.Map<UserMembershipResponseDto>(entity);
        }

        public async Task<List<UserMembershipResponseDto>> GetAllUserMembershipsAsync()
        {
            var entities = await _unitOfWork.UserMemberships.GetAllUserMembershipsAsync();
            return _mapper.Map<List<UserMembershipResponseDto>>(entities);
        }

        public async Task<(UserMembershipResponseDto?, string?)> AddUserMembershipAsync(AddUserMembershipRequestDto dto)
        {
            var entity = _mapper.Map<UserMembership>(dto);
            var success = await _unitOfWork.UserMemberships.AddUserMembershipAsync(entity);
            if (!success) return (null, "Failed to add UserMembership");

            await _unitOfWork.SaveChangesAsync();
            return (_mapper.Map<UserMembershipResponseDto>(entity), null);
        }

        public async Task<(UserMembershipResponseDto?, string?)> UpdateUserMembershipAsync(UpdateUserMembershipRequestDto dto)
        {
            var existing = await _unitOfWork.UserMemberships.GetUserMembershipByIdAsync(dto.UserMembershipId);
            if (existing == null) return (null, "UserMembership not found");

            _mapper.Map(dto, existing);
            var success = await _unitOfWork.UserMemberships.UpdateUserMembershipAsync(existing);
            if (!success) return (null, "Failed to update UserMembership");

            await _unitOfWork.SaveChangesAsync();
            return (_mapper.Map<UserMembershipResponseDto>(existing), null);
        }

        public async Task<string?> DeleteUserMembershipAsync(int id)
        {
            var success = await _unitOfWork.UserMemberships.DeleteUserMembershipAsync(id);
            if (!success) return "Failed to delete UserMembership";

            await _unitOfWork.SaveChangesAsync();
            return null;
        }
        public async Task<IEnumerable<UserMembershipResponseDto>> GetActiveMembershipsByUserAsync(int userId)
        {
            // Giả sử repository đã có FindAsync
            var entities = await _unitOfWork.UserMemberships
                              .FindAsync(um => um.UserId == userId && um.IsActive);

            // Map về DTO và trả về
            return _mapper.Map<IEnumerable<UserMembershipResponseDto>>(entities);
        }
    }
}
