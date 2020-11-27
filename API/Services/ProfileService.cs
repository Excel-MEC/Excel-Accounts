using System;
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
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;
        private readonly IEnvironmentService _env;

        public ProfileService(ICloudStorage cloudStorage, IConfiguration configuration, IEnvironmentService env)
        {
            _cloudStorage = cloudStorage;
            _configuration = configuration;
            _env = env;
        }

        public async Task<string> UploadProfileImage(DataForProfilePicUpdateDto data)
        {
            var fileNameForStorage = "accounts/profile/" + data.Name + Path.GetExtension(data.Image.FileName);
            await _cloudStorage.UploadFileAsync(data.Image, fileNameForStorage);
            var imageUrl = $"{_env.CloudStorageUrl}{fileNameForStorage}";
            return imageUrl;
        }
    }
}