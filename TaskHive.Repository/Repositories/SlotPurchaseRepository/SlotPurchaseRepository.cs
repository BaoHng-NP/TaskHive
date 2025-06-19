using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.SlotPurchaseRepository
{
    public class SlotPurchaseRepository : ISlotPurchaseRepository
    {
        private readonly AppDbContext _context;
        public SlotPurchaseRepository(AppDbContext context) => _context = context;

        public async Task<bool> AddAsync(SlotPurchase entity)
        {
            await _context.SlotPurchases.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(SlotPurchase entity)
        {
            _context.SlotPurchases.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.SlotPurchases.FindAsync(id);
            if (entity == null) return false;
            _context.SlotPurchases.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<SlotPurchase?> GetByIdAsync(int id)
        {
            return await _context.SlotPurchases.FirstOrDefaultAsync(x => x.SlotPurchaseId == id);
        }

        public async Task<List<SlotPurchase>> GetAllAsync()
        {
            return await _context.SlotPurchases.ToListAsync();
        }
    }
}
