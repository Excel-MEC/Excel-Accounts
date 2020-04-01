using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Dtos.Test;
using API.Models;

namespace API.Data.ProfileRepository
{
    public interface IProfileRepository
    {   
        Task<User> GetUser(int id);
        Task<bool> UpdateProfile(User user, DataForProfileUpdateDto data);
        Task<bool> UpdateProfileImage(User user, DataForFileUploadDto data);

    }
}