using Microsoft.AspNetCore.Mvc;
using StockApi.Domain.Interfaces;
using StockApi.Domain.Entities;
using StockApi.Response;
using Microsoft.AspNetCore.Authorization;

namespace StockApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        [HttpGet("get-all-categories")]
        public async Task<ActionResult<ApiResponse<List<Category>>>> GetAllCategories()
        {
            var allCategories = await _categoryRepository.GetAllCategories();
            if (allCategories is null)
                return Failure<List<Category>>(new List<string> { "All categories fetched" }, "Categories fetched successfully");

            return Success(allCategories, "Categories fetched successfully");
        }

        [HttpGet("get-category-by-id/{categoryId}")]
        public async Task<ActionResult<ApiResponse<Category>>> GetCategoryById(string categoryId)
        {
            var category = await _categoryRepository.GetCategoryById(categoryId);
            if (category == null)          
                return NotFoundResponse<Category>(new List<string> {"Category not found" },"Error finding Category");
            
            return Success(category,"Categories fetched successfully");
        }


        [HttpPost("create-category")]
        public async Task<ActionResult<ApiResponse<Category>>> CreateCategory(Category categoryData)
        {
            if (categoryData is null)
               return Failure<Category>(new List<string> { "Invalid order data" }, "Error creating order");
            

            var createdCategory = await _categoryRepository.CreateCategory(categoryData);

            return CreatedAtAction(nameof(GetCategoryById), new { categoryId = createdCategory.Id }, Success(createdCategory,"Category created successfully"));
        }


        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(string categoryId)
        {
            if (categoryId is null)
            {
                return NotFoundResponse<bool>(new List<string> { $"Invalid category ID {categoryId}" }, "Error finding category");
            }
            var isDeleted = await _categoryRepository.DeleteCategory(categoryId);
            if (!isDeleted)
                return Failure<bool>(new List<string> { "Category not found or could not be deleted" },"Error deleting category");

            return Success(isDeleted, "Category deleted successfully");
        }
    }
}
