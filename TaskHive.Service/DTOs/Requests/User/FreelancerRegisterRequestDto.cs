using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.User
{
    public class FreelancerRegisterRequestDto : RegisterRequestDto
    {
        
        public string? PortfolioUrl { get; set; }

        public List<int> SkillIds { get; set; } = new List<int>();
    }
}