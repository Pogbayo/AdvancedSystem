namespace StockApi.Domain.Entities
{
    public class OrderItem
    {
        public required string Id { get; set; }
        public required string OrderId { get; set; }
        public required string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
 