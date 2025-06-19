using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskHive.Repository.Entities;
using TaskHive.Repository.UnitOfWork;
using TaskHive.Service.DTOs.Requests.Category;
using TaskHive.Service.DTOs.Responses;

namespace TaskHive.Service.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(categoryId);
            return category == null ? null : _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllCategoriesAsync();
            return _mapper.Map<List<CategoryResponseDto>>(categories);
        }

        public async Task<(CategoryResponseDto? category, string? error)> AddCategoryAsync(AddCategoryRequestDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.IsDeleted = false;

            var success = await _unitOfWork.Categories.AddCategoryAsync(category);
            if (!success) return (null, "Failed to add category");

            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.Categories.GetCategoryByIdAsync(category.CategoryId);
            return (_mapper.Map<CategoryResponseDto>(result), null);
        }

        public async Task<(CategoryResponseDto? category, string? error)> UpdateCategoryAsync(UpdateCategoryRequestDto categoryDto)
        {
            var existingCategory = await _unitOfWork.Categories.GetCategoryByIdAsync(categoryDto.CategoryId);
            if (existingCategory == null) return (null, "Category not found");

            _mapper.Map(categoryDto, existingCategory);

            var success = await _unitOfWork.Categories.UpdateCategoryAsync(existingCategory);
            if (!success) return (null, "Failed to update category");

            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.Categories.GetCategoryByIdAsync(existingCategory.CategoryId);
            return (_mapper.Map<CategoryResponseDto>(result), null);
        }

        public async Task<string?> DeleteCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(categoryId);
            if (category == null) return "Category not found";

            var success = await _unitOfWork.Categories.DeleteCategoryAsync(categoryId);
            if (!success) return "Failed to delete category";

            await _unitOfWork.SaveChangesAsync();
            return null;
        }
    }
}
