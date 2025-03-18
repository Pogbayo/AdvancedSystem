namespace StockApi.Domain.Entities
{
    public class CartItem
    {
        public required string Id { get; set; }
        public required string ProductId { get; set; } 
        public int Quantity { get; set; }
        public required string UserId { get; set; }
        public Product? Product { get; set; }  
        public User? User { get; set; }
    }
}
