using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.User
{
    public class GoogleRegisterRequestDto
    {
        [Required]
        public string IdToken { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!; 

        public string? PortfolioUrl { get; set; }

        public List<int>? SkillIds { get; set; }
    }
}