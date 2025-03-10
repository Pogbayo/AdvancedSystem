using StockApi.Models;

namespace StockApi.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();
        Task<Product?> GetProductById(string ProductId);
        Task AddProduct(Product product);
        Task<bool> UpdateProduct(string productId, Product updateProduct);
        Task<bool> DeleteProduct(string productId);
        
    }
}
