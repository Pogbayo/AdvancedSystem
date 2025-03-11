using MongoDB.Driver;
using StockApi.Interfaces;
using StockApi.Models;
namespace StockApi.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly IMongoCollection<CartItem> _cartItems;
        public CartItemRepository(IMongoDatabase database)
        {
            _cartItems = database.GetCollection<CartItem>("CartItems");
        }
        public async Task<bool> DeleteCartItem(string cartItemId)
        {
            if (string.IsNullOrEmpty(cartItemId))
            {
                throw new ArgumentException("Please provide an ID");
            }
            var result = await _cartItems.DeleteOneAsync(c => c.Id == cartItemId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<List<CartItem>> GetAllCartItems()
        {
            return await _cartItems.Find(cartItem => true).ToListAsync();
        }

        public async Task<CartItem> GetCartItemById(string cartItemId)
        {
            var cartItem = await _cartItems.Find(c => c.Id == cartItemId).FirstOrDefaultAsync();
            if (cartItem==null)
            {
                throw new Exception("CartItem does not exist");
            }
            return cartItem;
        }

        public Task<bool> UpdateQuantity(string cartItemId, int newQuantity)
        {
            
        }
    }
}
