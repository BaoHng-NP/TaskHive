namespace TaskHive.Service.Services.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmailAsync(string email, string fullName, string verificationToken);
        Task<bool> SendWelcomeEmailAsync(string email, string fullName);
        Task<bool> SendPasswordResetEmailAsync(string email, string fullName, string resetToken);
    }
}