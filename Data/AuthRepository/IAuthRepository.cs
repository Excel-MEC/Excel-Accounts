using System.Threading.Tasks;
using Excel_Accounts_Backend.Models;

namespace Excel_Accounts_Backend.Data.AuthRepository
{
    public interface IAuthRepository
    {
        Task<User> Register(User user);
        Task<bool> UserExists(string email);
        Task<User> GetUser(string email);
    }
}