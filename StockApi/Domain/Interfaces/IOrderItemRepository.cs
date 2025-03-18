using StockApi.Domain.Entities;

namespace StockApi.Domain.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItem>> GetAllOrderItems();
        Task<OrderItem> GetOrderItemById(string orderItemId);
        Task<bool> DeleteOrderItem(string orderItemId);
        Task<bool> UpdateOrderItem(string orderItemId, OrderItem orderItemData);
        Task<bool> UpdateQuantity(string orderItemId, int newQuantity);
        Task<bool> UpdatePrice(string orderItemId, decimal newPrice);
        Task<OrderItem> AddOrderItem(OrderItem newOrderItem);
        Task<bool> OrderItemExists(string orderItemId);
    }
}
