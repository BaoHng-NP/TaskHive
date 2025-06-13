using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.User
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}