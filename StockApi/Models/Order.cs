namespace StockApi.Models
{
    public class Order
    {
        public enum OrderStatusEnum
        {
            Pending,
            Shipped,
            Delivered,
            Cancelled
        }

        public required string Id { get; set; }
        public required  string UserId { get; set; }
        public DateTime OrderTime { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatusEnum OrderStatus { get; set; } = OrderStatusEnum.Pending;
        public List<OrderItem>? ItemList { get; set; }
    }
}
