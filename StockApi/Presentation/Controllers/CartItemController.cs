using Microsoft.AspNetCore.Mvc;
using StockApi.Domain.Interfaces;
using StockApi.Domain.Entities;
using StockApi.Response;
using Microsoft.AspNetCore.Authorization;

namespace StockApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartItemController : BaseController
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemController(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }



        [HttpGet("get-all-cart-items")]
        public async Task<ActionResult<ApiResponse<List<CartItem>>>> GetAllCartItems()
        {
            var allCartItems = await _cartItemRepository.GetAllCartItems();
            if (allCartItems == null)
            {
                return Failure<List<CartItem>>(new List<string> { "Cartitems not fetched successfully" }, "Errorfetching cartitems");
            }
            return Success(allCartItems, "CartItems fetched successfully");
        }


        [HttpGet("get-cartitem-by-id/{cartItemId}")]
        public async Task<ActionResult<ApiResponse<CartItem>>> GetCartItemById(string cartItemId)
        {
            if (string.IsNullOrEmpty(cartItemId))
            {
                return NotFoundResponse<CartItem>(new List<string> { "Invalid cartItem ID" }, "Error finding cart item");
            }

            var cartItem = await _cartItemRepository.GetCartItemById(cartItemId);
            if (cartItem == null)
            {
                return Failure<CartItem>(new List<string> { "CartItem not found" }, "Error finding cart item");
            }

            return Success(cartItem, "Cart item found successfully");
        }



        [HttpPatch("update-cartitem-quantity/{cartItemId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateCartItemQuantity(string cartItemId, int newQuantity)
        {
            if (string.IsNullOrEmpty(cartItemId) || newQuantity <= 0)
            {
                return Failure<bool>(new List<string> { "Invalid cart item data" }, "Error updating cart item quantity");
            }

            var cartItem = await _cartItemRepository.GetCartItemById(cartItemId);
            if (cartItem == null)
            {
                return NotFoundResponse<bool>(new List<string> { "Cart item not found" }, "Error updating quantity");
            }

            var isUpdated = await _cartItemRepository.UpdateQuantity(cartItemId, newQuantity);
            if (!isUpdated)
            {
                return Failure<bool>(new List<string> { "Quantity not updated" }, "Error updating quantity");
            }

            return Success(true, "Cart item quantity updated successfully");
        }



        [HttpDelete("delete-cart-item/{cartItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCartItem(string cartItemId)
        {
            if (string.IsNullOrEmpty(cartItemId))
            {
                return Failure<bool>(new List<string> { "Invalid cart item ID" }, "Error deleting cart item");
            }

            var cartItem = await _cartItemRepository.GetCartItemById(cartItemId);
            if (cartItem == null)
            {
                return NotFoundResponse<bool>(new List<string> { "Cart item not found" }, "Error deleting cart item");
            }

            var isDeleted = await _cartItemRepository.DeleteCartItem(cartItemId);
            if (!isDeleted)
            {
                return Failure<bool>(new List<string> { "Cart item not deleted" }, "Error deleting cart item");
            }

            return Success(isDeleted, "Cart item deleted successfully");
        }
      }
    }
