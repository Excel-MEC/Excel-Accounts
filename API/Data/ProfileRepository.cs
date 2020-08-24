using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Models;
using Microsoft.Extensions.Configuration;
using API.Services.Interfaces;
using API.Data.Interfaces;
using API.Dtos.Admin;
using API.Extensions.CustomExceptions;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Users
            .Include(user => user.Ambassador)
            .Include(user => user.Referrer)
            .FirstOrDefaultAsync(user => user.Id == userid);
        }

        public async Task<List<User>> GetUserList(List<int> userIds)
        {
            return await _context.Users.Where(user => userIds.Contains(user.Id)).ToListAsync();
        }

        public async Task<bool> UpdateProfile(int id, UserForProfileUpdateDto data)
        {            
            User user = await _context.Users.FindAsync(id);
            user.Name = data.Name ?? user.Name;
            user.Gender = data.Gender ?? user.Gender;
            user.MobileNumber = data.MobileNumber ?? user.MobileNumber;            
            var categoryId = data.CategoryId ?? user.CategoryId.ToString();
            var institutionId = data.InstitutionId ?? user.InstitutionId;
            user.CategoryId = int.Parse(categoryId);            
            if(categoryId == "2")
            {
                user.InstitutionId = null;
                return await _context.SaveChangesAsync() > 0;
            }
            if (institutionId == 0) //Adds new college or school
            {
                if (user.Category == "college")
                {
                    var college = await _institution.AddCollege(data.InstitutionName);
                    user.InstitutionId = college.Id;
                }
                else if (user.Category == "school")
                {
                    var school = await _institution.AddSchool(data.InstitutionName);
                    user.InstitutionId = school.Id;
                }
            }
            else 
            {
                user.InstitutionId = institutionId;               
            }
            return await _context.SaveChangesAsync() > 0;
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

        public async Task<bool> ChangeRole(DataForChangingRoleDto dataForChangingRoleDto)
        {
            var user = await _context.Users.FindAsync(dataForChangingRoleDto.Id);
            user.Role = dataForChangingRoleDto.Role;
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }

        public async Task<string> GetRole(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user.Role;
        }
    }
}