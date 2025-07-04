// TaskHive.API/Controllers/CategoryController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskHive.Service.DTOs.Requests.Category;
using TaskHive.Service.DTOs.Responses;
using TaskHive.Service.Services.CategoryService;

namespace TaskHive.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(typeof(CategoryResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            return category == null ? NotFound() : Ok(category);
        }

        [HttpGet]
        [HttpHead]
        [ProducesResponseType(typeof(List<CategoryResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddCategory(AddCategoryRequestDto categoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (category, error) = await _categoryService.AddCategoryAsync(categoryDto);
            if (error != null) return BadRequest(error);

            return CreatedAtAction(nameof(GetCategoryById), new { categoryId = category?.CategoryId }, category);
        }

        [HttpPut]
        [ProducesResponseType(typeof(CategoryResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryRequestDto categoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (category, error) = await _categoryService.UpdateCategoryAsync(categoryDto);
            if (error != null) return BadRequest(error);
            if (category == null) return NotFound();

            return Ok(category);
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var error = await _categoryService.DeleteCategoryAsync(categoryId);
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }
}