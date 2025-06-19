using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Service.DTOs.Responses.User
{
    public class UserResponseDto
    {
    }
        public class UserSkillDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryDescription { get; set; }
    }


    public class FreelancerProfileResponseDto
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; }
        public string? UserName { get; set; }
        public string? CVFile { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? Country { get; set; }
        public string? imageUrl { get; set; }

        public bool? IsEmailVerified { get; set; }

        public List<UserSkillDto> Skills { get; set; } = new();
    }

    public class ClientProfileResponseDto
    {
        public string Email { get; set; } = null!;

        public string FullName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? CompanyDescription { get; set; }
        public string? Country { get; set; }
        public string? imageUrl { get; set; }
        public bool? IsEmailVerified { get; set; }
    }
}
