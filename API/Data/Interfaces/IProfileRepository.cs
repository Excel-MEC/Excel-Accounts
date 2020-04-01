using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Dtos.Test;
using API.Models;

namespace API.Data.Interfaces
{
    public interface IProfileRepository
    {
        Task<User> GetUser(int id);
        Task<bool> UpdateProfile(int id, UserForProfileUpdateDto data);
        Task<bool> UpdateProfileImage(int id, string imageUrl);

    }
}