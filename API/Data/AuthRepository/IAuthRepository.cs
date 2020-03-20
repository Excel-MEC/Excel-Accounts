using System.Threading.Tasks;
using API.Models;

namespace API.Data.AuthRepository
{
    public interface IAuthRepository
    {
        Task<User> Register(User user);
        Task<bool> UserExists(string email);
        Task<User> GetUser(string email);
    }
}