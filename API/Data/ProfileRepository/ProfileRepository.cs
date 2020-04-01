using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Dtos.Test;
using API.Data.InstitutionRepository;
using API.Models;
using API.Data.CloudStorage;
using System.IO;
using System;
using Microsoft.Extensions.Configuration;

namespace API.Data.ProfileRepository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly DataContext _context;
        private readonly IInstitutionRepository _institution;
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;
        public ProfileRepository(DataContext context, IInstitutionRepository institution, ICloudStorage cloudStorage, IConfiguration configuration)
        {
            this._configuration = configuration;
            this._cloudStorage = cloudStorage;
            _institution = institution;
            _context = context;
        }
        public async Task<User> GetUser(int userid)
        {
            return await _context.Users.FindAsync(userid);
        }

        public async Task<bool> UpdateProfile(User user, DataForProfileUpdateDto data)
        {
            user.Name = data.Name;
            user.Gender = data.Gender;
            user.MobileNumber = data.MobileNumber;
            user.Category = data.Category;
            if (data.InstitutionId == 0)
            {
                if (data.Category == "college")
                {
                    var college = await _institution.AddCollege(data.InstitutionName);
                    user.InstitutionId = college.Id;
                }
                else if( data.Category == "school")
                {    
                    var school = await _institution.AddSchool(data.InstitutionName);
                    user.InstitutionId = school.Id;
                }
            }
            else
            {
                user.InstitutionId = data.InstitutionId;   
            }
               
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        } 
        

        public async Task<bool> UpdateProfileImage(User user, DataForFileUploadDto data)
        {
            await UploadFile(data);
            string ImageUrl = _configuration.GetValue<string>("CloudStorageUrl") + data.ImageStorageName;
            user.Picture = ImageUrl;

            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }

        private async Task UploadFile(DataForFileUploadDto dataForFileUpload)
        {
            string fileNameForStorage = "accounts/qr-code/" + FormFileName(dataForFileUpload.Name, dataForFileUpload.Image.FileName);
            dataForFileUpload.ImageUrl = await _cloudStorage.UploadFileAsync(dataForFileUpload.Image, fileNameForStorage);
            dataForFileUpload.ImageStorageName = fileNameForStorage;
        }

        private static string FormFileName(string title, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileNameForStorage = $"{title}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtension}";
            return fileNameForStorage;

        }
    }
}