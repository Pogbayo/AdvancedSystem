using StockApi.Domain.Entities;

namespace StockApi.Domain.Interfaces
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetAllCartItems();
        Task<CartItem> GetCartItemById(string cartItemId);
        Task<bool> UpdateQuantity(string cartItemId, int newQuantity);
        Task<bool> DeleteCartItem(string cartItemId);
    }
}
