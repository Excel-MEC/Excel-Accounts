using System.Collections.Generic;
using System.Threading.Tasks;
using Excel_Accounts_Backend.Models;

namespace Excel_Accounts_Backend.Data.InstitutionRepository
{
    public interface IInstitutionRepository
    {
       Task<bool> AddCollege(string Name); 
       Task<bool> AddSchool(string Name);
       Task<List<College>> CollegeList();
       Task<List<School>> SchoolList();


    }
}