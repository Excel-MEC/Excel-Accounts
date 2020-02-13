using System.Threading.Tasks;
using Excel_Accounts_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Excel_Accounts_Backend.Data.ProfileRepository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly DataContext _context;
        public ProfileRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> GetUser(int userid)
        {
            return await _context.Users.FindAsync(userid);
        }
    }
}