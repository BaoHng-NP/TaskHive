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

        [Required]
        public string Country { get; set; } = null!;

        public UserRole Role { get; set; } = UserRole.Freelancer;

        public bool IsEmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public ICollection<ConversationMember> ConversationMemberships { get; set; } = new List<ConversationMember>();
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Conversation> CreatedConversations { get; set; } = new List<Conversation>();

    }

    public class Freelancer : User
    {
        public int RemainingSlots { get; set; }
        public string? CVFile { get; set; }
        public string? PortfolioUrl { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
        public ICollection<UserMembership> Memberships { get; set; } = new List<UserMembership>();
        public ICollection<SlotPurchase> SlotPurchases { get; set; } = new List<SlotPurchase>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public class Client : User
    {
        public string CompanyName { get; set; } = null!;
        public string? CompanyWebsite { get; set; }
        public string? CompanyDescription { get; set; }
        public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
    }
}
