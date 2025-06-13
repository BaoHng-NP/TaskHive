namespace TaskHive.Service.DTOs.Responses
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class EmailVerificationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? RedirectUrl { get; set; }
    }

    public class PasswordResetResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }
}