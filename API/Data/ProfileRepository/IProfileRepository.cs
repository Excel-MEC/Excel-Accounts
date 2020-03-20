using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Data.ProfileRepository
{
    public interface IProfileRepository
    {   
        Task<User> GetUser(int id);
        Task<bool> UpdateProfile(User user, DataForProfileUpdateDto data);
    }
}