using Microsoft.EntityFrameworkCore;
using StockApi.Models;

namespace StockApi.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserById(string userId);
        Task<bool> UpdateUser(string userId, User updateUser);
        Task<bool> DeleteUser(string userid);
        Task<bool> SignUp(User user);
        Task<User?> Login(string email, string password);
    }
}
