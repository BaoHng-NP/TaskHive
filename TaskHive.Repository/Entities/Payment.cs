using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Enums;

namespace TaskHive.Repository.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Freelancer User { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentType PaymentType { get; set; }

        public int? MembershipId { get; set; }

        [ForeignKey("MembershipId")]
        public Membership? Membership { get; set; }

        public int? SlotPurchaseId { get; set; }

        [ForeignKey("SlotPurchaseId")]
        public SlotPurchase? SlotPurchase { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }
    }
}
