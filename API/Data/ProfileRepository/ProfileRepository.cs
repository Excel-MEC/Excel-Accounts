using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Data.InstitutionRepository;
using API.Models;
using System;

namespace API.Data.ProfileRepository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly DataContext _context;
        private readonly IInstitutionRepository _institution;
        public ProfileRepository(DataContext context, IInstitutionRepository institution)
        {
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
            user.IsCollege = data.IsCollege;    
            if (data.InstitutionId == 0)
            {
                if (data.IsCollege)
                {
                    var collegeId = await _institution.AddCollege(data.InstitutionName);
                    user.InstitutionId = collegeId;  
                }
                else
                {
                    var school = new School();
                    await _institution.AddSchool(data.InstitutionName);
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
    }
}