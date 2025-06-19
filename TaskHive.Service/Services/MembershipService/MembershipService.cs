using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            bool nameExists = await _unitOfWork.Memberships
            .AnyAsync(m => m.Name.ToLower() == membershipDto.Name.ToLower() && !m.IsDeleted);

            if (nameExists)
            {
                return (null, "Membership name already exists.");
            }
            var membership = _mapper.Map<Membership>(membershipDto);
            membership.IsDeleted = false;
            membership.Status = true;
            membership.CreatedAt = DateTime.UtcNow.AddHours(7);

            if (membershipDto.MonthlySlotLimit < 0)
                return (null, "Slot limit must be non-negative.");

            var success = await _unitOfWork.Memberships.AddMembershipAsync(membership);
            if (!success) return (null, "Failed to add membership");

            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.Memberships.GetMembershipByIdAsync(membership.MembershipId);
            return (_mapper.Map<MembershipResponseDto>(result), null);
        }

        public async Task<(MembershipResponseDto? membership, string? error)> UpdateMembershipAsync(UpdateMembershipRequestDto membershipDto)
        {
            // 1. Tìm membership cần sửa
            var existing = await _unitOfWork.Memberships.GetMembershipByIdAsync(membershipDto.MembershipId);
            if (existing == null)
                return (null, "Membership not found");

            // 2. Kiểm tra trùng tên nếu tên mới khác tên cũ
            if (!string.IsNullOrWhiteSpace(membershipDto.Name) &&
                !string.Equals(membershipDto.Name, existing.Name, StringComparison.OrdinalIgnoreCase))
            {
                var nameExists = await _unitOfWork.Memberships
                    .AnyAsync(m => m.Name.ToLower() == membershipDto.Name!.ToLower() &&
                                   m.MembershipId != existing.MembershipId &&
                                   !m.IsDeleted);

                if (nameExists)
                    return (null, "Membership name already exists.");
            }

            // 3. Map các thuộc tính cần cập nhật từ DTO vào entity
            _mapper.Map(membershipDto, existing);

            // 4. Gọi hàm update
            var success = await _unitOfWork.Memberships.UpdateMembershipAsync(existing);
            if (!success)
                return (null, "Failed to update membership");

            await _unitOfWork.SaveChangesAsync();

            // 5. Lấy lại bản ghi vừa update và trả về DTO
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
