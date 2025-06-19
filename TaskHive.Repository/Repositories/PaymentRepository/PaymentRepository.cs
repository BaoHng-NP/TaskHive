using Microsoft.EntityFrameworkCore;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments.ToListAsync();
        }
    }
}
