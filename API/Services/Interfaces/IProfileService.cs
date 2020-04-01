using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Dtos.Test;

namespace API.Services.Interfaces
{
    public interface IProfileService
    {
        Task<string> UploadProfileImage(DataForProfilePicUpdateDto data);
    }
}