using Microsoft.AspNetCore.Mvc;
using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        [HttpGet("get-all-categories")]
        public async Task<ActionResult<List<Category>>> GetAllCategories()
        {
            return Ok(await _categoryRepository.GetAllCategories());
        }

        [HttpGet("get-category-by-id/{categoryId}")]
        public async Task<ActionResult<Category>> GetCategoryById(string categoryId)
        {
            var category = await _categoryRepository.GetCategoryById(categoryId);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }
            return Ok(category);
        }


        [HttpPost("create-category")]
        public async Task<ActionResult<Category>> CreateCategory(Category categoryData)
        {
            var createdCategory = await _categoryRepository.CreateCategory(categoryData);
            return CreatedAtAction(nameof(GetCategoryById), new { categoryId = createdCategory.Id }, createdCategory);
        }


        [HttpDelete("{categoryId}")]
        public async Task<ActionResult> DeleteCategory(string categoryId)
        {
            var isDeleted = await _categoryRepository.DeleteCategory(categoryId);
            if (!isDeleted)
            {
                return NotFound(new { message = "Category not found or could not be deleted" });
            }
            return Ok(new { message = "Category deleted successfully" });
        }
    }
}
