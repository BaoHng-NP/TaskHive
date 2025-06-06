using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Repository.Entities
{
    public class BlogPost
    {
        [Key]
        public int BlogpostId { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; } = null!;

        [Required]
        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string? Status { get; set; }

        public bool IsDeleted { get; set; }
    }
}
