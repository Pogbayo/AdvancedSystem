using MongoDB.Driver;
using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders;
        public OrderRepository(IMongoDatabase orders)
        {
            _orders = orders.GetCollection<Order>("Orders");
        }

        Task<bool> IOrderRepository.AddItemToOrder(string orderId, OrderItem item)
        {
            throw new NotImplementedException();
        }

        Task<Order> IOrderRepository.CreateOrder(Order orderData)
        {
            throw new NotImplementedException();
        }

        Task<bool> IOrderRepository.DeleteItemFromOrder(string orderId, string itemId)
        {
            throw new NotImplementedException();
        }

        Task<bool> IOrderRepository.DeleteOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        Task<List<Order>> IOrderRepository.GetAllOrders()
        {
            throw new NotImplementedException();
        }

        Task<Order> IOrderRepository.GetOrderById(string orderId)
        {
            throw new NotImplementedException();
        }

        Task<bool> IOrderRepository.UpdateOrderStatus(string orderId, Order.OrderStatusEnum newStatus)
        {
            throw new NotImplementedException();
        }

        Task<bool> IOrderRepository.UpdateOrderTotalAmount(string orderId, decimal newTotalAmount)
        {
            throw new NotImplementedException();
        }
    }
}
