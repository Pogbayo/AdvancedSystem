using Microsoft.AspNetCore.Mvc;
using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemController(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        [HttpGet("get-all-cart-items")]
        public async Task<ActionResult<List<CartItem>>> GetAllCartItems()
        {
            return Ok(await _cartItemRepository.GetAllCartItems());
        }


        [HttpGet("get-cartitem-by-id/{cartItemId}")]
        public async Task<IActionResult> GetCartItemById(string cartItemId)
        {
            var cartItem = await _cartItemRepository.GetCartItemById(cartItemId);
            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }
            return Ok(cartItem);
        }

        [HttpPatch("update-cartitem-quantity/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(string cartItemId, int newQuantity)
        {
            var cartItem = await _cartItemRepository.GetCartItemById(cartItemId);
            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }
            var isUpdated = await _cartItemRepository.UpdateQuantity(cartItemId, newQuantity);
            if (!isUpdated)
            {
                return BadRequest(false);
            }
            return Ok(true);
        }


        [HttpDelete("delete-cart-item/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(string cartItemId)
        {
            var cartItem = await _cartItemRepository.GetCartItemById(cartItemId);
            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }
            var isDeleted = await _cartItemRepository.DeleteCartItem(cartItemId);
            return Ok(true);
        }
    }
}
