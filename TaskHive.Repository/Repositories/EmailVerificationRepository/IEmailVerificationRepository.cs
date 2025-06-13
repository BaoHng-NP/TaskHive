using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.EmailVerificationRepository
{
    public interface IEmailVerificationRepository
    {
        Task<EmailVerificationToken?> GetByTokenAsync(string token);
        Task<bool> HasValidTokenAsync(int userId);
        Task AddAsync(EmailVerificationToken token);
        Task UpdateAsync(EmailVerificationToken token);

        // Password reset methods (new)
        Task<EmailVerificationToken?> GetPasswordResetTokenAsync(string token);
        Task<bool> HasValidPasswordResetTokenAsync(int userId);
        Task InvalidatePasswordResetTokensAsync(int userId);
    }
}