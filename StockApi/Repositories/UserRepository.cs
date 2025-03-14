using Microsoft.AspNetCore.Http.HttpResults;
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


        public async Task<User> Register(User newUser)
        {

            newUser.Password = HashPassword(newUser.Password);
            await _users.InsertOneAsync(newUser);
            return newUser;
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
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user==null)
            {
                throw new KeyNotFoundException("User not found");
            }

            if (newRole != "Admin" && newRole != "User")
            {
                throw new ArgumentException("Invalid role");
            }

            if (!user.Roles.Contains(newRole))
            {
                user.Roles.Add(newRole);
                await _users.ReplaceOneAsync(u => u.Id == id, user);
            }
            return user;

        }

        public async Task<User> GetUserByEmail(string userEmail)
        {
            return await _users.Find(u => u.Email.ToLower() == userEmail.ToLower()).FirstOrDefaultAsync();
            //if (user == null)
            //{
            //    throw new KeyNotFoundException($"User with email {userEmail} not found");
            //}

        }
    }
}
