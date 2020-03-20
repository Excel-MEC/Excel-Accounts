using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Data.InstitutionRepository
{
    public interface IInstitutionRepository
    {
       Task<bool> AddCollege(string Name); 
       Task<bool> AddSchool(string Name);
       Task<List<College>> CollegeList();
       Task<List<School>> SchoolList();


    }
}