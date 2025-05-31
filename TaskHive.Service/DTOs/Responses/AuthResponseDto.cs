namespace TaskHive.Service.DTOs.Responses
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!; 
        public string Role { get; set; } = null!;
        public string? Message { get; set; } 
    }
}