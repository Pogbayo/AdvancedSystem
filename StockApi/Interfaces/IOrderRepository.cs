using StockApi.Models;
namespace StockApi.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrders();
        Task<string> CreateOrder(Order orderData);
        Task<Order> GetOrderById(string orderId);
        Task<bool> DeleteOrder(string orderId);
        Task<bool> UpdateOrderStatus(string orderId, Order.OrderStatusEnum newStatus);
        Task<bool> UpdateOrderTotalAmount(string orderId, decimal newTotalAmount);
        Task<bool> AddItemToOrder(string orderId, OrderItem item);
        Task<bool> DeleteItemFromOrder(string orderId, string itemId);
    }
}
