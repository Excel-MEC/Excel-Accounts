using System.Threading.Tasks;
using Excel_Accounts_Backend.Dtos.Profile;
using Excel_Accounts_Backend.Data.InstitutionRepository;
using Excel_Accounts_Backend.Models;

namespace Excel_Accounts_Backend.Data.ProfileRepository
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
            user.Gender = data.Gender;
            user.MobileNumber = data.MobileNumber;
            if (data.InstitutionId == 0)
            {
                if (data.IsCollege)
                {
                    var college = new College();
                    await _institution.AddCollege(data.InstitutionName);
                    user.InstitutionId = college.Id;
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