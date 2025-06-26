using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Repositories.BlogPostRepository;
using TaskHive.Service.DTOs.Requests.BlogPost;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.DTOs.Responses.BlogPost;

namespace TaskHive.Service.Services.BlogPostService
{
    public interface IBlogPostService
    {
        Task<PagedResult<BlogPostResponseDto>> GetPagedBlogPostsAsync(PostQueryParam parameters);
        Task<BlogPostResponseDto?> GetBlogPostByIdAsync(int id);
        Task<BlogPostResponseDto?> CreateBlogPostAsync(BlogPostRequestDto request, int authorId);
        Task<BlogPostResponseDto?> UpdateBlogPostAsync(UpdateBlogPostDto request, int authorId);
        Task<bool> DeleteBlogPostAsync(int id, int authorId);
        Task<List<BlogPostResponseDto>> GetAllBlogPostsAsync();
    }
}
