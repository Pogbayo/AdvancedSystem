using MongoDB.Driver;
using StockApi.Domain.Interfaces;
using StockApi.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace StockApi.Infrastructure.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        
        public  async Task<List<User>> GetAllUsers()
        {
            var usersWithDetails = await _users.Aggregate()
                .Lookup("orders", "Id", "userId", "Orders")
                .Lookup("cartitems", "Id", "UserId", "CartItems")
                .As<User>()
                .ToListAsync();
           
            return usersWithDetails;
        }



        public async Task<User?> GetUserById(string userId)
        {
            var user = await _users.Aggregate()
                .Match(u => u.Id == userId)
                .Lookup("orders", "Id", "userId", "Orders")
                .Lookup("cartitems", "Id", "UserId", "CartItems")
                .As<User>()
                .FirstOrDefaultAsync();

            if (user==null)
                throw new KeyNotFoundException($"User with ID {userId} not found");
            
            return user;
        }



        public async Task<User?> Login(string email, string password)
        {
            var user = await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
            if (user == null || !VerifyPassword(password, user.Password))
            {
                return null;
            }
            return user;
        }


        public async Task<User> Register(User newUser)
        {
            newUser.Password = HashPassword(newUser.Password);
            await _users.InsertOneAsync(newUser);

            var user = await _users.Aggregate()
              .Match(u => u.Id == newUser.Id)
              .Lookup("orders", "Id", "userId", "Orders")
              .Lookup("cartitems", "Id", "UserId", "CartItems")
              .As<User>()
              .FirstOrDefaultAsync();

            return user;
        }



        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            return HashPassword(enteredPassword) == storedHashedPassword;
        }


        public async Task<bool> UpdateUser(string userId, User updateUser)
        {
            var result = await _users.ReplaceOneAsync(u => u.Id == userId, updateUser);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUser(string userid)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == userid);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }


        public async Task<User> UpdateUserRole(string id, string newRole)
        {

            if (newRole != "Admin" && newRole != "User")
            {
                throw new ArgumentException("Invalid role");
            }

            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user==null)
            {
                throw new KeyNotFoundException("User not found");
            }

            if (!user.Roles.Contains(newRole))
            {
                var update = Builders<User>.Update.AddToSet(u => u.Roles, newRole);
                await _users.UpdateOneAsync(u => u.Id == id, update);

                user.Roles.Add(newRole);
            }

            return user;

        }

        public async Task<User> GetUserByEmail(string userEmail)
        {
            var user = await _users.Aggregate()
                        .Match(u => u.Email == userEmail)
                        .Lookup("orders", "Id", "UserId", "Orders")
                        .Lookup("cartItems", "Id", "UserId", "CartItems")
                        .As<User>()
                        .FirstOrDefaultAsync();

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userEmail} not found");

            return user;

        }
    }
}
