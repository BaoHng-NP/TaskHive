using System.ComponentModel.DataAnnotations;
using TaskHive.Repository.Enums;

namespace TaskHive.Service.DTOs.Requests.Payment
{
    public class AddPaymentWithSlotRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentType PaymentType { get; set; }

        [Required]
        public int SlotPurchaseId { get; set; }
        public decimal? SlotQuantity { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }
    }
}
