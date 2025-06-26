using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.BlogPostRepository
{
    public interface IBlogPostRepository
    {
        Task<List<BlogPost>> GetAllAsync();
        Task<List<BlogPost>> GetAllValidAsync();

        Task<BlogPost?> GetByIdAsync(int id);
        Task CreateAsync(BlogPost blogPost);
        Task UpdateAsync(BlogPost blogPost);
        Task<bool> DeleteAsync(int id);
        Task<List<BlogPost>> GetPagedAsync(PostQueryParam parameters);
    }
}
