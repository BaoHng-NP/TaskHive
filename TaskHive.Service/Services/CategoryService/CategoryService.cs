using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.Repositories.CategoryRepository;
using TaskHive.Service.DTOs.Requests.Category;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            return category == null ? null : _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<List<CategoryResponseDto>>(categories);
        }

        public async Task<(CategoryResponseDto? category, string? error)> AddCategoryAsync(AddCategoryRequestDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.IsDeleted = false;

            var success = await _categoryRepository.AddCategoryAsync(category);
            if (!success) return (null, "Failed to add category");

            var result = await _categoryRepository.GetCategoryByIdAsync(category.CategoryId);
            return (_mapper.Map<CategoryResponseDto>(result), null);
        }

        public async Task<(CategoryResponseDto? category, string? error)> UpdateCategoryAsync(UpdateCategoryRequestDto categoryDto)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(categoryDto.CategoryId);
            if (existingCategory == null) return (null, "Category not found");

            _mapper.Map(categoryDto, existingCategory);

            var success = await _categoryRepository.UpdateCategoryAsync(existingCategory);
            if (!success) return (null, "Failed to update category");

            var result = await _categoryRepository.GetCategoryByIdAsync(existingCategory.CategoryId);
            return (_mapper.Map<CategoryResponseDto>(result), null);
        }

        public async Task<string?> DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null) return "Category not found";

            var success = await _categoryRepository.DeleteCategoryAsync(categoryId);
            return success ? null : "Failed to delete category";
        }
    }
}