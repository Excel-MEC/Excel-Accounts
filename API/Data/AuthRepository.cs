using System.Threading.Tasks;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Data.Interfaces;

namespace API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context; 
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> GetUser(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
            return user;
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