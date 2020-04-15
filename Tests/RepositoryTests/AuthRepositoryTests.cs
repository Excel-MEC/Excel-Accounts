using System.Threading.Tasks;
using API.Data;
using API.Data.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.RepositoryTests
{
    public class AuthRepositoryTests : IClassFixture<DbContextFixture>
    {
        private readonly DataContext _context;
        private readonly IAuthRepository _repo;
        public AuthRepositoryTests(DbContextFixture dbContextFixture)
        {
            _context = dbContextFixture.Context;
            _repo = new AuthRepository(_context);
        }
        [Fact]
        public async Task GetUser_GivenValidEmail_ReturnsUserAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync();
            var userFromRepo = await _repo.GetUser(user.Email);
            Assert.Equal(user.Email, userFromRepo.Email);
        }

        [Fact]
        public async Task Register_GivenUserObject_ReturnsUserAsync()
        {
            var newUser = new User
            {
                Name = "Register User",
                Email = "register@mail.com",
                Picture = "registerpictureurl.png",
                QRCodeUrl = "registerQRcode.png",
                InstitutionId = 1,
                Gender = "male",
                MobileNumber = "1234567890",
                Category = "college"
            };
            var userFromRepo = await _repo.Register(newUser);
            var userFromContext = await _context.Users.FirstOrDefaultAsync(x => x.Email == newUser.Email);
            Assert.Equal(userFromRepo, userFromContext);
        }

        [Fact]
        public async Task UserExists_GivenEmail_ReturnsTrueOrFalseAsync()
        {
            var userFromContext = await _context.Users.FirstOrDefaultAsync();
            var response = await _repo.UserExists(userFromContext.Email);
            Assert.True(response);
            response = await _repo.UserExists("notexitsting@email.com");
            Assert.False(response);
        }
    }
}