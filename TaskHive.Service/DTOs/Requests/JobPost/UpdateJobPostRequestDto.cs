using System.ComponentModel.DataAnnotations;

namespace TaskHive.Service.DTOs.Requests.JobPost
{
    public class UpdateJobPostRequestDto
    {
        [Required]
        public int JobPostId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? JobType { get; set; }
        public string? Status { get; set; }
        public DateTime? Deadline { get; set; }
    }
}