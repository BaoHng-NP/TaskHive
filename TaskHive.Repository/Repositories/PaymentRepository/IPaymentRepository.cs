using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.PaymentRepository
{
    public interface IPaymentRepository
    {
        Task<bool> AddPaymentAsync(Payment payment);
        Task<List<Payment>> GetAllPaymentsAsync();
    }
}
