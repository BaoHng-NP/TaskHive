using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;

namespace TaskHive.Repository.Repositories.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<bool> AddCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}