using Logko.API.Models;
namespace Logko.API.Data;

public interface IUserRepo
{
    Task<User> CreateUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> UserExistsAsync(string username);
}