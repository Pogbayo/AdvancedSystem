using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApi.Domain.Interfaces;
using StockApi.Domain.Entities;
using StockApi.Response;

namespace StockApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Product>>>> GetAllProducts()
        {
            var products = await _productRepository.GetAllProducts();
            if (products == null || !products.Any())
            {
                return NotFoundResponse<List<Product>>(new List<string> { "No product found" }, "Products not fetched successfully");
            }
            return Success(products, "Products fetched successfully");
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<ApiResponse<Product>>> GetProductById(string productId)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFoundResponse<Product>(new List<string> { "Product not found" }, "Failed to fetch product");
            }
            return Success(product, "Product fetched successfully");
        }

        [HttpPost("add-product")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Product>>> AddProduct(Product product, IFormFile file)
        {
            var addedProduct = await _productRepository.AddProduct(product,file);
            if (addedProduct == null)
            {
                return Failure<Product>(new List<string> { "Failed to add product" }, "Add product operation failed");
            }
            return Success(addedProduct, "Product added successfully");
        }

        [HttpPut("{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateProduct(string productId, Product productData)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFoundResponse<bool>(new List<string> { "Product not found" }, "Update operation failed");
            }

            var updatedProduct = await _productRepository.UpdateProduct(productId, productData);
            if (!updatedProduct)
            {
                return Failure<bool>(new List<string> { "Failed to update product" }, "Update operation failed");
            }

            return Success(true, "Product updated successfully");
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(string productId)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFoundResponse<bool>(new List<string> { "Product not found" }, "Delete operation failed");
            }

            var isDeleted = await _productRepository.DeleteProduct(productId);
            if (!isDeleted)
            {
                return Failure<bool>(new List<string> { "Failed to delete product" }, "Delete operation failed");
            }

            return Success(true, "Product deleted successfully");
        }
    }
}
