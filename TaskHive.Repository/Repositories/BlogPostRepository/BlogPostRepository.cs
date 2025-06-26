using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.BlogPostRepository
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly AppDbContext _context;
        public BlogPostRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(BlogPost blogPost)
        {
            await _context.BlogPosts.AddAsync(blogPost);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null) return false;

            blogPost.IsDeleted = true;
            _context.BlogPosts.Update(blogPost);
            return true;
        }

        public async Task<List<BlogPost>> GetAllAsync()
        {
            return await _context.BlogPosts
                .Include(bp => bp.Author)
                .Where(bp => !bp.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetAllValidAsync()
        {
            return await _context.BlogPosts
                .Include(bp => bp.Author)
                .Where(BlogPost => !BlogPost.IsDeleted && BlogPost.PublishedAt != null)
                .OrderByDescending(bp => bp.PublishedAt)
                .ToListAsync();
        }


        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            return await _context.BlogPosts
                .Include(bp => bp.Author)
                .FirstOrDefaultAsync(bp => bp.BlogpostId == id && !bp.IsDeleted);
        }

        public async Task<List<BlogPost>> GetPagedAsync(PostQueryParam parameters)
        {
            var query = _context.BlogPosts
                .Include(bp => bp.Author)
                .Where(bp => !bp.IsDeleted);

            if (!string.IsNullOrEmpty(parameters.Search))
            {
                query = query.Where(bp => bp.Title.Contains(parameters.Search) || bp.Content.Contains(parameters.Search));
            }

            if (parameters.IsPublished.HasValue)
            {
                if (parameters.IsPublished.Value)
                    query = query.Where(bp => bp.PublishedAt != null);
                else
                    query = query.Where(bp => bp.PublishedAt == null);
            }

            return await query
                .OrderByDescending(bp => bp.Title)
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();
        }

        public async Task UpdateAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Update(blogPost);
        }
    }
}
