using MongoDB.Driver;
using StockApi.Domain.Entities;
using StockApi.Domain.Interfaces;

namespace StockApi.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly ImageService _imageService;
        public ProductRepository(IMongoDatabase database, ImageService imageService)
        {
            _products = database.GetCollection<Product>("Products");
            _imageService = imageService; 
        }


        public async Task<List<Product>> GetAllProducts()
        {
            var queriedProducts = await _products.Aggregate()
                .Lookup("categories", "CategoryId", "Id", "Category")
                .As<Product>()
                .ToListAsync();

            return queriedProducts;
        }


        public async Task<Product?> GetProductById(string productId)
        {
            var product = await _products.Aggregate()
                     .Match(u => u.Id == productId)
                     .Lookup("categories", "CategoryId", "Id", "Category")
                     .As<Product>()
                     .FirstOrDefaultAsync();

            if (product == null)
                throw new KeyNotFoundException($"User with ID {productId} not found");

            return product;

        }

        public async Task<Product> AddProduct(Product product, IFormFile file)
        {
            if (string.IsNullOrEmpty(product.ImgUrl))
            {
                throw new ArgumentException("Product Img url is required");
            }

            string imageUrl = await _imageService.UploadImage(file); 
            product.ImgUrl = imageUrl; 

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
