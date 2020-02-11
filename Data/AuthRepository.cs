using System.Threading.Tasks;
using Excel_Accounts_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Excel_Accounts_Backend.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Register(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Users.AnyAsync(user => user.Email == email))
            {
                return true;
            }

            return false;
        }
    }
}