using MongoDB.Driver;
using StockApi.Interfaces;
using StockApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace StockApi.Repositories
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
            return await _users.Find(u => true).ToListAsync();
        }



        public async Task<User?> GetUserById(string userId)
        {
            var user = await _users.Find(user => user.Id == userId).FirstOrDefaultAsync();
            if (user==null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }
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


        public async Task<bool> SignUp(User user)
        {
            var existingUser = await _users.Find(u => user.Email == user.Email).FirstOrDefaultAsync();
            if (existingUser!= null)
            {
                return false;
            }
            user.Password = HashPassword(user.Password);
            await _users.InsertOneAsync(user);
            return true;
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
    }
}

