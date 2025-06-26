using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.DTOs.Requests.BlogPost
{
    public class BlogPostRequestDto
    {

        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string? Status { get; set; } //Dùng status lưu imageURL

        public bool? IsDeleted { get; set; }=false; 
    }

    public class UpdateBlogPostDto : BlogPostRequestDto
    {
        public int Id { get; set; }
    }
}
