using System.Threading.Tasks;
using Excel_Accounts_Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Excel_Accounts_Backend.Data.ProfileRepository
{
    public interface IProfileRepository
    {
        Task<User> GetUser(int id);
    }
}