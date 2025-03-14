using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace StockApi.Models
{
    public class User
    {
        [BsonId]  
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public  string? UserName { get; set; }
        [BsonElement("email")]
        public required string Email { get; set; }
        [BsonElement("password")]
        public required string Password { get; set; }

        public List<string> Roles { get; set; } = new List<string> { "User" };  
    }
}
