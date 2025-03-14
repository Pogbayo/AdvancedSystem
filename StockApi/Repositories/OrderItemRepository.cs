using EllipticCurve.Utils;
using MongoDB.Driver;
using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IMongoCollection<OrderItem> _orderItems;


        public OrderItemRepository(IMongoDatabase database)
        {
            _orderItems = database.GetCollection<OrderItem>("OrderItems");
        }


        public async Task<OrderItem> AddOrderItem(OrderItem newOrderItem)
        {
            var newOrder = newOrderItem;
            if (newOrder == null)
            {
                throw new ArgumentException("Order structure not complete");
            }
            await _orderItems.InsertOneAsync(newOrder);
            return newOrder;
        }

        public async Task<bool> DeleteOrderItem(string orderItemId)
        {
            var result = await _orderItems.DeleteOneAsync(or => or.Id == orderItemId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }


        public async Task<List<OrderItem>> GetAllOrderItems()
        {
            return await _orderItems.Find(orderItem => true).ToListAsync();
        }


        public async Task<OrderItem> GetOrderItemById(string orderItemId)
        {
            if (!await OrderItemExists(orderItemId))
            {
                throw new KeyNotFoundException("Order does not exist");
            }
            var orderItem = await _orderItems.Find(or=>or.Id==orderItemId).FirstOrDefaultAsync();
            return orderItem;
        }



        public async Task<bool> OrderItemExists(string orderItemId)
        {
            return await _orderItems.Find(o => o.Id == orderItemId).AnyAsync();
        }


        public async Task<bool> UpdateOrderItem(string orderItemId, OrderItem orderItemData)
        {
            if (orderItemData == null)
            {
                return false;
            }
            var result = await _orderItems.ReplaceOneAsync(or => or.Id == orderItemId, orderItemData);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }



        public async Task<bool> UpdatePrice(string orderItemId, decimal newPrice)
        {
            var update = Builders<OrderItem>.Update.Set(o => o.Price, newPrice);
            var result = await _orderItems.UpdateOneAsync(o => o.Id == orderItemId, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;

        }



        public async Task<bool> UpdateQuantity(string orderItemId, int newQuantity)
        {
            var update = Builders<OrderItem>.Update.Set(o => o.Quantity, newQuantity);
            var result = await _orderItems.UpdateOneAsync(o => o.Id == orderItemId, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
