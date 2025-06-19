using TaskHive.Repository.Enums;

namespace TaskHive.Service.DTOs.Responses
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentType PaymentType { get; set; }
        public int? MembershipId { get; set; }
        public int? SlotPurchaseId { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
