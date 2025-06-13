namespace TaskHive.Service.DTOs.Responses
{
    public class ApplicationResponseDto
    {
        public int ApplicationId { get; set; }
        public int FreelancerId { get; set; }
        public string FreelancerName { get; set; } = null!;
        public int JobPostId { get; set; }
        public string JobPostTitle { get; set; } = null!;
        public string? CoverLetter { get; set; }
        public decimal? BidAmount { get; set; }
        public string? Status { get; set; }
        public string? CVFile { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}