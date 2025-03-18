using StockApi.Domain.Entities;

namespace StockApi.Domain.Interfaces

{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();
        Task<Product?> GetProductById(string ProductId);
        Task<Product> AddProduct(Product product, IFormFile file);
        Task<bool> UpdateProduct(string productId, Product updateProduct);
        Task<bool> DeleteProduct(string productId);
        
    }
}
