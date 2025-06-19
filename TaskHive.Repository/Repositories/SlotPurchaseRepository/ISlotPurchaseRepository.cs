using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.SlotPurchaseRepository
{
    public interface ISlotPurchaseRepository
    {
        Task<bool> AddAsync(SlotPurchase entity);
        Task<bool> UpdateAsync(SlotPurchase entity);
        Task<bool> DeleteAsync(int id);
        Task<SlotPurchase?> GetByIdAsync(int id);
        Task<List<SlotPurchase>> GetAllAsync();
    }
}
