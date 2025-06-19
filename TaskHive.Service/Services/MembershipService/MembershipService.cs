using AutoMapper;
using TaskHive.Repository.Entities;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.Membership;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.MembershipService
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MembershipResponseDto?> GetMembershipByIdAsync(int membershipId)
        {
            var membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(membershipId);
            return membership == null ? null : _mapper.Map<MembershipResponseDto>(membership);
        }

        public async Task<List<MembershipResponseDto>> GetAllMembershipsAsync()
        {
            var memberships = await _unitOfWork.Memberships.GetAllMembershipsAsync();
            return _mapper.Map<List<MembershipResponseDto>>(memberships);
        }

        public async Task<(MembershipResponseDto? membership, string? error)> AddMembershipAsync(AddMembershipRequestDto membershipDto)
        {
            var membership = _mapper.Map<Membership>(membershipDto);
            membership.IsDeleted = false;

            var success = await _unitOfWork.Memberships.AddMembershipAsync(membership);
            if (!success) return (null, "Failed to add membership");

            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.Memberships.GetMembershipByIdAsync(membership.MembershipId);
            return (_mapper.Map<MembershipResponseDto>(result), null);
        }

        public async Task<(MembershipResponseDto? membership, string? error)> UpdateMembershipAsync(UpdateMembershipRequestDto membershipDto)
        {
            var existing = await _unitOfWork.Memberships.GetMembershipByIdAsync(membershipDto.MembershipId);
            if (existing == null) return (null, "Membership not found");

            _mapper.Map(membershipDto, existing);

            var success = await _unitOfWork.Memberships.UpdateMembershipAsync(existing);
            if (!success) return (null, "Failed to update membership");

            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.Memberships.GetMembershipByIdAsync(existing.MembershipId);
            return (_mapper.Map<MembershipResponseDto>(result), null);
        }

        public async Task<string?> DeleteMembershipAsync(int membershipId)
        {
            var membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(membershipId);
            if (membership == null) return "Membership not found";

            var success = await _unitOfWork.Memberships.DeleteMembershipAsync(membershipId);
            if (!success) return "Failed to delete membership";

            await _unitOfWork.SaveChangesAsync();
            return null;
        }
    }
}
