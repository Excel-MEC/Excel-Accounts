using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers;
using API.Data.Interfaces;
using API.Dtos.Profile;
using API.Models;
using API.Models.Custom;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.ControllerTests
{
    public class ProfileControllerTests
    {
        private readonly Mock<IProfileRepository> _repo;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IInstitutionRepository> _institution;
        private readonly Mock<IProfileService> _profileService;
        private readonly ProfileController _profileController;
        private readonly User _user;

        public ProfileControllerTests()
        {
            _repo = new Mock<IProfileRepository>();
            _mapper = new Mock<IMapper>();
            _institution = new Mock<IInstitutionRepository>();
            _profileService = new Mock<IProfileService>();
            _profileController = new ProfileController(_repo.Object, _mapper.Object, _institution.Object, _profileService.Object);
            _user = Mock.Of<User>();
            _user.Id = 123;
            _user.Email = "a@b.com";
            var principalUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("user_id", _user.Id.ToString()),
                new Claim("email", _user.Email),
            }, "mock"));

            _profileController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principalUser }
            };
        }

        [Fact]
        public async Task Get_GivenNoArgeuments_ReturnCurrentUserAsync()
        {
            //When
            _repo.Setup(x => x.GetUser(_user.Id)).ReturnsAsync(_user);

            //Then
            var response = await _profileController.Get();
            Assert.IsType<ActionResult<User>>(response);
        }

        [Fact]
        public async Task UpdateProfile_GivenUserData_ReturnsSuccessAsync()
        {
            //Given
            var data = Mock.Of<UserForProfileUpdateDto>();
            //When
            _repo.Setup(x => x.UpdateProfile(_user.Id, data)).ReturnsAsync(true);
            //Then
            var response = await _profileController.UpdateProfile(data);
            var okObjectResult = response as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var okResponse = okObjectResult.Value as OkResponse;
            Assert.Equal("Success", okResponse.Response.ToString());
        }
    }
}