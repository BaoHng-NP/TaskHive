using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Entities
{
    public class JobPost
    {
        [Key]
        public int JobPostId { get; set; }

        [Required]
        public int EmployerId { get; set; }

        [ForeignKey("EmployerId")]
        public Client Employer { get; set; } = null!;

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        public string? Location { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public string? JobType { get; set; }

        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? Deadline { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}
