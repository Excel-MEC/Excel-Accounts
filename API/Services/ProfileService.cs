using System.IO;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Profile;
using API.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IInstitutionRepository _institution;
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;

        public ProfileService(IInstitutionRepository institution, ICloudStorage cloudStorage, IConfiguration configuration)
        {
            _institution = institution;
            _cloudStorage = cloudStorage;
            _configuration = configuration;
        }

        public async Task<string> UploadProfileImage(DataForProfilePicUpdateDto data)
        {
            string fileNameForStorage = "accounts/profile/" + data.Name + Path.GetExtension(data.Image.FileName);
            await _cloudStorage.UploadFileAsync(data.Image, fileNameForStorage);
            string imageUrl = _configuration.GetValue<string>("CloudStorageUrl") + fileNameForStorage;
            return imageUrl;
        }
    }
}