using MongoDB.Driver;
using StockApi.Domain.Entities;
using StockApi.Domain.Interfaces;

namespace StockApi.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMongoCollection<Category> _categories;
        public CategoryRepository(IMongoDatabase database)
        {
            _categories = database.GetCollection<Category>("Categories");
        }
        public async Task<Category> CreateCategory(Category categoryData)
        {
            await _categories.InsertOneAsync(categoryData);
            return categoryData;
        }

        public async Task<bool> DeleteCategory(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                throw new ArgumentException("CategoryId cannot be null or empty");
            }
            var result = await _categories.DeleteOneAsync(d => d.Id == categoryId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _categories.Find(category => true).ToListAsync();
        }

        public async Task<Category> GetCategoryById(string categoryId)
        {
            var category = await _categories.Find(c => c.Id == categoryId).FirstOrDefaultAsync();
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            return category;
        }
    }
}
