namespace TaskHive.Service.DTOs.Responses
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class EmailVerificationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? RedirectUrl { get; set; }
    }

    namespace TaskHive.Service.DTOs.Responses
{
    public class PasswordResetResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }
}
}