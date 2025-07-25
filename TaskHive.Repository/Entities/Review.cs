using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Entities
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int ReviewerId { get; set; }

        [ForeignKey("ReviewerId")]
        public User Reviewer { get; set; } = null!;

        [Required]
        public int RevieweeId { get; set; }

        [ForeignKey("RevieweeId")]
        public User Reviewee { get; set; } = null!;

        
        public int? JobPostId { get; set; }

        [ForeignKey("JobPostId")]
        public JobPost JobPost { get; set; } = null!;

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
