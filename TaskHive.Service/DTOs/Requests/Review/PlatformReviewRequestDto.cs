using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.Review
{
    public class PlatformReviewRequestDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string Comment { get; set; } = null!;
    }
}