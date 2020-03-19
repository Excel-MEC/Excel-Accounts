using System.Threading.Tasks;
using Excel_Accounts_Backend.Models;

namespace Excel_Accounts_Backend.Data.InstitutionRepository
{
    public class InstitutionRepository : IInstitutionRepository
    {
        private readonly DataContext _context;
        public InstitutionRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<bool> AddCollege(string Name)
        {
            var college = new College();
            college.Name = Name;    
            await _context.Colleges.AddAsync(college);
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }

        public async Task<bool> AddSchool(string Name)
        {
            var newschool = new School();
            newschool.Name = Name;
            await _context.Schools.AddAsync(newschool);
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }   
    }
}