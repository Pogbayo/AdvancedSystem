using System.ComponentModel.DataAnnotations;

namespace StockApi.Models
{
    public class User
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
