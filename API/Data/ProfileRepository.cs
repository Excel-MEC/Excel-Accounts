using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Models;
using Microsoft.Extensions.Configuration;
using API.Services.Interfaces;
using API.Data.Interfaces;

namespace API.Data
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly DataContext _context;
        private readonly IInstitutionRepository _institution;
        private readonly ICloudStorage _cloudStorage;
        private readonly IConfiguration _configuration;
        public ProfileRepository(DataContext context, IInstitutionRepository institution, ICloudStorage cloudStorage, IConfiguration configuration)
        {
            _configuration = configuration;
            _cloudStorage = cloudStorage;
            _institution = institution;
            _context = context;
        }
        public async Task<User> GetUser(int userid)
        {
            return await _context.Users.FindAsync(userid);
        }

        public async Task<bool> UpdateProfile(int id, UserForProfileUpdateDto data)
        {
            User user = await _context.Users.FindAsync(id);
            user.Name = data.Name;
            user.Gender = data.Gender;
            user.MobileNumber = data.MobileNumber;
            user.Category = data.Category;
            int institutionId = data.InstitutionId ?? default(int);
            string institutionName = data.InstitutionName ?? default(string);

            if (institutionId == 0)
            {
                if (data.Category == "college")
                {
                    var college = await _institution.AddCollege(institutionName);
                    user.InstitutionId = college.Id;
                }
                else if (data.Category == "school")
                {
                    var school = await _institution.AddSchool(institutionName);
                    user.InstitutionId = school.Id;
                }
            }
            else
            {
                user.InstitutionId = institutionId;
            }

            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }


        public async Task<bool> UpdateProfileImage(int id, string imageUrl)
        {
            var user = await _context.Users.FindAsync(id);
            if (user.Picture.Equals(imageUrl))
            {
                return true;
            }
            user.Picture = imageUrl;
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }

    }
}