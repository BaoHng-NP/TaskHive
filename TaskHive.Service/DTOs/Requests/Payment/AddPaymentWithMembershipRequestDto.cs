using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskHive.Repository.Enums;

namespace TaskHive.Service.DTOs.Requests.Payment
{
    public class AddPaymentWithMembershipRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType PaymentType { get; set; }

        [Required]
        public int MembershipId { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentStatus Status { get; set; }
    }
}
