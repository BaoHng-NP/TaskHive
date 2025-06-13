namespace TaskHive.Service.DTOs.Responses
{
    public class JobPostResponseDto
    {
        public int JobPostId { get; set; }
        public int EmployerId { get; set; }
        public string EmployerName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? JobType { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? Deadline { get; set; }
    }
}