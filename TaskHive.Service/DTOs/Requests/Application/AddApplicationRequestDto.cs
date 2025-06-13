using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.Application
{
    public class AddApplicationRequestDto
    {
        [Required]
        public int FreelancerId { get; set; }
        [Required]
        public int JobPostId { get; set; }
        public string? CoverLetter { get; set; }
        public decimal? BidAmount { get; set; }
        public string? Status { get; set; }
        public string? CVFile { get; set; }
    }
}