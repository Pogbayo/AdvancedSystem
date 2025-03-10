namespace StockApi.Models
{
    public class OrderItem
    {
        public required string Id { get; set; }
        public required string OrderId { get; set; }
        public required string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
