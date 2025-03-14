using MongoDB.Driver;
using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        public ProductRepository(IMongoDatabase database)
        {
            _products = database.GetCollection<Product>("Products");
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _products.Find(product => true).ToListAsync();
        }

        public async Task<Product?> GetProductById(string productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product==null)
            {
                throw new KeyNotFoundException($"Product with ID{productId} not found");
            }
            return product;
        }

        public async Task<Product> AddProduct(Product product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> UpdateProduct(string productId, Product updateProduct)
        {
            var result = await _products.ReplaceOneAsync(p => p.Id == productId, updateProduct);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProduct(string productId)
        {
            var result = await _products.DeleteOneAsync(p => p.Id == productId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
