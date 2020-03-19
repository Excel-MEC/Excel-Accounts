using System.Threading.Tasks;
using Excel_Accounts_Backend.Models;

namespace Excel_Accounts_Backend.Data.InstitutionRepository
{
    public interface IInstitutionRepository
    {
       Task<bool> AddCollege(string Name); 
       Task<bool> AddSchool(string Name); 
    }
}