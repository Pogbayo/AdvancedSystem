using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var products = await _productRepository.GetAllProducts();
            return products;
        }



        [HttpGet("{productId}")]
        public async Task<ActionResult<Product?>> GetProductById(string productId)
        {
            return Ok(await _productRepository.GetProductById(productId));
        }



        [HttpPost("add-product")]
        public async Task<ActionResult> AddProduct(Product product)
        {
            var addedProduct = await _productRepository.AddProduct(product);
            return Ok(addedProduct);
        }



        [HttpPut("{productId}")]
        public async Task<ActionResult<bool>> UpdateProduct(string productId, Product productData)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product==null)
            {
                return false;
            }
            var updatedProduct = await _productRepository.UpdateProduct(productId, productData);
            return Ok(true);
        }



        [HttpDelete("{productId}")]
        public async Task<ActionResult<bool>> DeleteProduct(string productId)
        {
            var product = await _productRepository.GetProductById(productId);
            if (product == null)
            {
                return false;
            }
            var updatedProduct = await _productRepository.DeleteProduct(productId);
            return Ok(true);
        }
    }
}
