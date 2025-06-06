using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Entities
{
    public class SlotPurchase
    {
        [Key]
        public int SlotPurchaseId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Freelancer User { get; set; } = null!;

        [Required]
        public int PurchasedSlots { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public DateTime PurchasedAt { get; set; }

        public bool IsActive { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
