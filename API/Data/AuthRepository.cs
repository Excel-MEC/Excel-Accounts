using System.Threading.Tasks;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Services.Interfaces;
using API.Data.Interfaces;

namespace API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IQRCodeGeneration _qRCodeGeneration;

        public AuthRepository(DataContext context, IQRCodeGeneration qRCodeGeneration)
        {
            _qRCodeGeneration = qRCodeGeneration;
            _context = context;
        }

        public async Task<User> GetUser(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
            return user;
        }

        public async Task<User> Register(User user)
        {
            user.QRCodeUrl = await _qRCodeGeneration.CreateQrCode(user.Id.ToString());
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