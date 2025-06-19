using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs.Requests.SlotPurchase;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.SlotPurchaseService
{
    public interface ISlotPurchaseService
    {
        Task<SlotPurchaseResponseDto?> GetByIdAsync(int id);
        Task<List<SlotPurchaseResponseDto>> GetAllAsync();
        Task<(SlotPurchaseResponseDto?, string?)> AddAsync(AddSlotPurchaseRequestDto dto);
        Task<(SlotPurchaseResponseDto?, string?)> UpdateAsync(UpdateSlotPurchaseRequestDto dto);
        Task<string?> DeleteAsync(int id);
    }
}
