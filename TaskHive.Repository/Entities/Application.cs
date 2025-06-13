using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Entities
{
    public class Application
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        public int FreelancerId { get; set; }

        [ForeignKey("FreelancerId")]
        public Freelancer Freelancer { get; set; } = null!;

        [Required]
        public int JobPostId { get; set; }

        [ForeignKey("JobPostId")]
        public JobPost JobPost { get; set; } = null!;

        public string? CoverLetter { get; set; }

        public decimal? BidAmount { get; set; }

        public string? Status { get; set; }

        public string? CVFile { get; set; }

        public DateTime AppliedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
