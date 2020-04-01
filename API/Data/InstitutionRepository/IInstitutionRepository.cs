using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Data.InstitutionRepository
{
    public interface IInstitutionRepository
    {
       Task<College> AddCollege(string Name); 
       Task<School> AddSchool(string Name);
       Task<List<College>> CollegeList();
       Task<List<School>> SchoolList();
       Task<string> FindName(string category, int id);
       
    }
}