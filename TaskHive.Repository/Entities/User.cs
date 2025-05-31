using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Enums;

namespace TaskHive.Repository.Entities
{
    public class User
    {

        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? PasswordHash { get; set; }

        public string? UserName { get; set; }
        public string? GoogleId { get; set; }
        public string? imageUrl { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        public UserRole Role { get; set; } = UserRole.Freelancer;

        public bool IsEmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
