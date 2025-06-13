using Microsoft.EntityFrameworkCore;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.EmailVerificationRepository
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        private readonly AppDbContext _context;

        public EmailVerificationRepository(AppDbContext context)
        {
            _context = context;
        }



        public async Task<EmailVerificationToken?> GetByTokenAsync(string token)
        {
            return await _context.EmailVerificationTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token &&
                                   !t.IsPasswordReset && // Email verification only
                                   !t.IsUsed &&
                                   t.ExpiresAt > DateTime.UtcNow);
        }
        public async Task<EmailVerificationToken?> GetActiveTokenByUserIdAsync(int userId)
        {
            return await _context.EmailVerificationTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task AddAsync(EmailVerificationToken token)
        {
            await _context.EmailVerificationTokens.AddAsync(token);
        }

        public async Task UpdateAsync(EmailVerificationToken token)
        {
            _context.EmailVerificationTokens.Update(token);
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var expiredTokens = await _context.EmailVerificationTokens
                .Where(t => t.ExpiresAt <= DateTime.UtcNow || t.IsUsed)
                .ToListAsync();

            _context.EmailVerificationTokens.RemoveRange(expiredTokens);
        }




        public async Task<EmailVerificationToken?> GetPasswordResetTokenAsync(string token)
        {
            return await _context.EmailVerificationTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token &&
                                   t.IsPasswordReset && // Password reset only
                                   !t.IsUsed &&
                                   t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<bool> HasValidTokenAsync(int userId)
        {
            return await _context.EmailVerificationTokens
                .AnyAsync(t => t.UserId == userId &&
                          !t.IsPasswordReset && // Email verification only
                          !t.IsUsed &&
                          t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<bool> HasValidPasswordResetTokenAsync(int userId)
        {
            return await _context.EmailVerificationTokens
                .AnyAsync(t => t.UserId == userId &&
                          t.IsPasswordReset && // Password reset only
                          !t.IsUsed &&
                          t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task InvalidatePasswordResetTokensAsync(int userId)
        {
            var tokens = await _context.EmailVerificationTokens
                .Where(t => t.UserId == userId &&
                       t.IsPasswordReset && // Password reset only
                       !t.IsUsed)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsUsed = true;
            }

            _context.EmailVerificationTokens.UpdateRange(tokens);
        }

    }
}