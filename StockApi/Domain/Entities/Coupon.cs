using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockApi.Domain.Entities
{
    public class Coupon
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ExpiryDate { get; set; }
    }
}
