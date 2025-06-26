using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs.Responses.User;

namespace TaskHive.Service.DTOs.Responses.BlogPost
{
    public class BlogPostResponseDto
    {
        public int BlogpostId { get; set; }

        public int AuthorId { get; set; }

        public UserResponseDto Author { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string? Status { get; set; }

        public bool IsDeleted { get; set; }
    }
}
