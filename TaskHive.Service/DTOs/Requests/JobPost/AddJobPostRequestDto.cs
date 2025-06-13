using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.JobPost
{
    public class AddJobPostRequestDto
    {
        [Required]
        public int EmployerId { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string? Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? JobType { get; set; }
        public string? Status { get; set; }
        public DateTime? Deadline { get; set; }
    }
}