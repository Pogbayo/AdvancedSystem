using MongoDB.Driver;
using MongoDB.Driver.Linq;
using StockApi.Domain.Entities;
using StockApi.Domain.Interfaces;

namespace StockApi.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {


        private readonly IMongoCollection<Order> _orders;
        public OrderRepository(IMongoDatabase orders)
        {
            _orders = orders.GetCollection<Order>("Orders");
        }


        public async Task<bool> AddItemToOrder(string orderId, OrderItem item)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new Exception("Order not found!");
            }
            if (order.ItemList == null)
            {
                order.ItemList = new List<OrderItem>();
            }

            order.ItemList.Add(item);

            var result = await _orders.UpdateOneAsync(
                o => o.Id == orderId,
                Builders<Order>.Update.Set
                (o => o.ItemList, order.ItemList));

;            return result.IsAcknowledged && result.ModifiedCount > 0;
        }



        public async Task<string> CreateOrder(Order orderData)
        {
              await _orders.InsertOneAsync(orderData);
            return "Order created successfully";
            
        }



        public async Task<bool> DeleteItemFromOrder(string orderId, string itemId)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            if (order.ItemList == null || order.ItemList.Count == 0)
            {
                throw new Exception("Item list is empty!");
            }

            var items = order.ItemList;
            if (items.Count == 0)
            {
                throw new Exception("itemsList is empty!");
            }

            var item = order.ItemList.FirstOrDefault(i => i.Id == itemId);
            if (item==null)
            {
                throw new Exception("item does not exist");
            }

            order.ItemList.Remove(item);

            var update = Builders<Order>.Update.Set(o => o.ItemList, order.ItemList);
            var result = await _orders.UpdateOneAsync(i => i.Id == itemId,update);
            return result.IsAcknowledged && result.ModifiedCount > 0;

        }



        public async Task<bool> DeleteOrder(string orderId)
        {
            var result = await _orders.DeleteOneAsync(i => i.Id == orderId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }



        public async Task<List<Order>> GetAllOrders()
        {
            var orders = await _orders.Aggregate()
                .Lookup("users", "UserId", "Id", "User")
                .As<Order>()
                .ToListAsync();

            return orders;
        }


        public async Task<Order> GetOrderById(string orderId)
        {
            var order = await _orders.Aggregate()
                .Match(u => u.Id == orderId)
                .Lookup("users", "UserId", "Id", "User")
                .As<Order>()
                .FirstOrDefaultAsync();

            if (order == null)
                throw new Exception("Order not found");
            
            return order;
        }



        public async  Task<bool> UpdateOrderStatus(string orderId, Order.OrderStatusEnum newStatus)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            order.OrderStatus = newStatus;

            var update = Builders<Order>.Update.Set(o => o.OrderStatus, order.OrderStatus);
            var result = await _orders.UpdateOneAsync(o=>o.Id==order.Id,update);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }



        public async  Task<bool> UpdateOrderTotalAmount(string orderId, decimal newTotalAmount)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            order.TotalAmount = newTotalAmount;
            var update = Builders<Order>.Update.Set(o => o.TotalAmount, order.TotalAmount);
            var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
