using System.Threading.Tasks;
using API.Data;
using API.Data.Interfaces;
using API.Dtos.Profile;
using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Tests.RepositoryTests
{
    public class ProfileRepositoryTests : IClassFixture<DbContextFixture>
    {
        private readonly DataContext _context;
        private readonly IProfileRepository _repo;
        private readonly Mock<ICloudStorage> _cloudStorage;
        private readonly Mock<IInstitutionRepository> _institution;
        private readonly Mock<IConfiguration> _configuration;
        public ProfileRepositoryTests(DbContextFixture dbContextFixture)
        {
            _context = dbContextFixture.Context;
            _cloudStorage = new Mock<ICloudStorage>();
            _institution = new Mock<IInstitutionRepository>();
            _configuration = new Mock<IConfiguration>();
            _repo = new ProfileRepository(_context, _institution.Object, _cloudStorage.Object, _configuration.Object);
        }

        [Fact]
        public async Task GetUser_GivenUserId_ReturnsUserObjectAsync()
        {
            var userFromContext = await _context.Users.FirstOrDefaultAsync();
            var userFromRepo = await _repo.GetUser(userFromContext.Id);
            Assert.Equal(userFromContext.Email, userFromRepo.Email);
        }

        [Fact]
        public async Task UpdateUser_GivenIdAndUserProfileUpdateDto_ReturnsSuccessAsync()
        {
            var newUser = new User
            {
                Name = "Update User",
                Email = "updateuser@mail.com",
                Picture = "updateuserpictureurl.png",
                QRCodeUrl = "updateuserQRcode.png",
                InstitutionId = 1,
                Gender = "male",
                MobileNumber = "1234567890",
                Category = "college"
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            var userFromContext = await _context.Users.FirstOrDefaultAsync(x => x.Email == newUser.Email);
            var userForProfileUpdate = Mock.Of<UserForProfileUpdateDto>();
            userForProfileUpdate.Name = newUser.Name + " Update";
            userForProfileUpdate.InstitutionId = 2;
            userForProfileUpdate.Gender = "female";
            userForProfileUpdate.Category = "school";
            var success = await _repo.UpdateProfile(userFromContext.Id, userForProfileUpdate);
            Assert.True(success);
            userFromContext = await _context.Users.FirstOrDefaultAsync(x => x.Email == newUser.Email);
            Assert.Equal(userFromContext.Name, userForProfileUpdate.Name);
        }

        [Fact]
        public async Task UpdateProfileImage_GivenNewUrlAndUserId_ReturnsSuccessAsync()
        {
            var newUser = new User
            {
                Name = "Update Profile Image",
                Email = "updateprofileimage@mail.com",
                Picture = "updateuserpictureurl.png",
                QRCodeUrl = "updateuserQRcode.png",
                InstitutionId = 1,
                Gender = "male",
                MobileNumber = "1234567890",
                Category = "college"
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            var userFromContext = await _context.Users.FirstOrDefaultAsync(x => x.Email == newUser.Email);
            var newProfileImageUrl = "New Url";
            var success = await _repo.UpdateProfileImage(userFromContext.Id, newProfileImageUrl);
            Assert.True(success);
            userFromContext = await _context.Users.FirstOrDefaultAsync(x => x.Email == newUser.Email);
            Assert.Equal(newProfileImageUrl, userFromContext.Picture);
        }
    }
}