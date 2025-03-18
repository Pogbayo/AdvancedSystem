using StockApi.Domain.Entities;

namespace StockApi.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserById(string userId);
        Task<User> GetUserByEmail(string userEmail);
        Task<bool> UpdateUser(string userId, User updateUser);
        Task<bool> DeleteUser(string userid);
        Task<User> Register(User user);
        Task<User> UpdateUserRole(string userId, string newRole);
        Task<User?> Login(string email, string password);
    }
}
