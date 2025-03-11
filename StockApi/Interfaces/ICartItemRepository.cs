using StockApi.Models;

namespace StockApi.Interfaces
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetAllCartItems();
        Task<CartItem> GetCartItemById(string cartItemId);
        Task<bool> UpdateQuantity(string cartItemId, int newQuantity);
        Task<bool> DeleteCartItem(string cartItemId);
    }
}
