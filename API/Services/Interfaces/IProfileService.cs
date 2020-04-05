using System.Threading.Tasks;
using API.Dtos.Profile;

namespace API.Services.Interfaces
{
    public interface IProfileService
    {
        Task<string> UploadProfileImage(DataForProfilePicUpdateDto data);
    }
}