using StockApi.Domain.Entities;

namespace StockApi.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategories();
        Task<Category> GetCategoryById(string categoryId);
        Task<Category> CreateCategory(Category categoryData);
        Task<bool> DeleteCategory(string categoryId);
    }
}
