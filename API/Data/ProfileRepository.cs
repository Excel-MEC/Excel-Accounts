using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Dtos.Test;
using API.Models;
using System.IO;
using System;
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
            this._configuration = configuration;
            this._cloudStorage = cloudStorage;
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
            if (data.InstitutionId == 0)
            {
                if (data.Category == "college")
                {
                    var college = await _institution.AddCollege(data.InstitutionName);
                    user.InstitutionId = college.Id;
                }
                else if (data.Category == "school")
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


        public async Task<bool> UpdateProfileImage(int id, string imageUrl)
        {
            var user = await _context.Users.FindAsync(id);
            user.Picture = imageUrl;
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }

    }
}