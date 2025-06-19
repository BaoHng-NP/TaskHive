using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.SlotPurchase;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.SlotPurchaseService
{
    public class SlotPurchaseService : ISlotPurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SlotPurchaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SlotPurchaseResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.SlotPurchases.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<SlotPurchaseResponseDto>(entity);
        }

        public async Task<List<SlotPurchaseResponseDto>> GetAllAsync()
        {
            var list = await _unitOfWork.SlotPurchases.GetAllAsync();
            return _mapper.Map<List<SlotPurchaseResponseDto>>(list);
        }

        public async Task<(SlotPurchaseResponseDto?, string?)> AddAsync(AddSlotPurchaseRequestDto dto)
        {
            var entity = _mapper.Map<SlotPurchase>(dto);
            var success = await _unitOfWork.SlotPurchases.AddAsync(entity);
            if (!success) return (null, "Failed to add");

            await _unitOfWork.SaveChangesAsync();
            return (_mapper.Map<SlotPurchaseResponseDto>(entity), null);
        }

        public async Task<(SlotPurchaseResponseDto?, string?)> UpdateAsync(UpdateSlotPurchaseRequestDto dto)
        {
            var existing = await _unitOfWork.SlotPurchases.GetByIdAsync(dto.SlotPurchaseId);
            if (existing == null) return (null, "Not found");

            _mapper.Map(dto, existing);
            var success = await _unitOfWork.SlotPurchases.UpdateAsync(existing);
            if (!success) return (null, "Failed to update");

            await _unitOfWork.SaveChangesAsync();
            return (_mapper.Map<SlotPurchaseResponseDto>(existing), null);
        }

        public async Task<string?> DeleteAsync(int id)
        {
            var success = await _unitOfWork.SlotPurchases.DeleteAsync(id);
            if (!success) return "Failed to delete";
            await _unitOfWork.SaveChangesAsync();
            return null;
        }
    }
}
