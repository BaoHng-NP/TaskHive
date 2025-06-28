using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.Repositories.BlogPostRepository;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.BlogPost;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.DTOs.Responses.BlogPost;

namespace TaskHive.Service.Services.BlogPostService
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlogPostService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<BlogPostResponseDto>> GetPagedBlogPostsAsync(PostQueryParam parameters)
        {
            try
            {
                var (blogPosts,totalCount) = await _unitOfWork.BlogPosts.GetPagedAsync(parameters);

                var blogPostDtos = _mapper.Map<List<BlogPostResponseDto>>(blogPosts);

                return new PagedResult<BlogPostResponseDto>
                {
                    Items = blogPostDtos,
                    TotalItems = totalCount,
                    Page = parameters.Page,
                    PageSize = parameters.PageSize
                };

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching paged blog posts.", ex);
                return new PagedResult<BlogPostResponseDto>();
            }
        }


        public async Task<BlogPostResponseDto?> GetBlogPostByIdAsync(int id)
        {
            try
            {
                var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(id);
                return blogPost == null ? null : _mapper.Map<BlogPostResponseDto>(blogPost);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching the blog post with ID {id}.", ex);
                return null;
            }
        }

        public async Task<BlogPostResponseDto?> CreateBlogPostAsync(BlogPostRequestDto request, int authorId)
        {
            try
            {
                var blogPost = _mapper.Map<BlogPost>(request);
                blogPost.AuthorId = authorId;
                await _unitOfWork.BlogPosts.CreateAsync(blogPost);
                await _unitOfWork.SaveChangesAsync();

                var createdBlogPost = await _unitOfWork.BlogPosts.GetByIdAsync(blogPost.BlogpostId);
                return _mapper.Map<BlogPostResponseDto>(blogPost);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the blog post.", ex);
                return null;
            }
        }

        public async Task<BlogPostResponseDto?> UpdateBlogPostAsync(UpdateBlogPostDto request, int authorId)
        {
            try
            {
                var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(request.Id);
                if (blogPost == null)
                {
                    return null;
                }
                _mapper.Map(request, blogPost);
                await _unitOfWork.BlogPosts.UpdateAsync(blogPost);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<BlogPostResponseDto>(blogPost);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the blog post.", ex);
                return null;
            }
        }
        public async Task<bool> DeleteBlogPostAsync(int id, int authorId)
        {
            try
            {
                var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(id);
                if (blogPost == null || blogPost.AuthorId != authorId)
                {
                    return false;
                }
                blogPost.IsDeleted = true;
                await _unitOfWork.BlogPosts.UpdateAsync(blogPost);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the blog post with ID {id}.", ex);
                return false;
            }
        }
        public async Task<List<BlogPostResponseDto>> GetAllBlogPostsAsync()
        {
            try
            {
                var blogPosts = await _unitOfWork.BlogPosts.GetAllAsync();
                return _mapper.Map<List<BlogPostResponseDto>>(blogPosts);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching all blog posts.", ex);
                return new List<BlogPostResponseDto>();
            }

        }
    }
}
