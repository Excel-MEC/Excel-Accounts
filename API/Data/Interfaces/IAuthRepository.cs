using System.Threading.Tasks;
using API.Models;

namespace API.Data.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> Register(User user);
        Task<bool> UserExists(string email);
        Task<User> GetUser(string email);
        Task<User> GetUserById(int userId);
    }
}