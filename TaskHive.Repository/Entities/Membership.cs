using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Entities
{
    public class Membership
    {
        [Key]
        public int MembershipId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int MonthlySlotLimit { get; set; }

        public string? Features { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
