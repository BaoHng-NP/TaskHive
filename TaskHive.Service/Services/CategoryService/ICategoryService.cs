    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskHive.Service.DTOs.Requests.Category;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int categoryId);
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<(CategoryResponseDto? category, string? error)> AddCategoryAsync(AddCategoryRequestDto categoryDto);
        Task<(CategoryResponseDto? category, string? error)> UpdateCategoryAsync(UpdateCategoryRequestDto categoryDto);
        Task<string?> DeleteCategoryAsync(int categoryId);
    }
}