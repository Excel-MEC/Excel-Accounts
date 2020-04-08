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
        private readonly ProfileController _controller;
        private readonly User _user;

        public ProfileControllerTests()
        {
            _repo = new Mock<IProfileRepository>();
            _mapper = new Mock<IMapper>();
            _institution = new Mock<IInstitutionRepository>();
            _profileService = new Mock<IProfileService>();
            _controller = new ProfileController(_repo.Object, _mapper.Object, _institution.Object, _profileService.Object);
            _user = Mock.Of<User>();
            _user.Id = 123;
            _user.Email = "a@b.com";
            _user.InstitutionId = 1;
            var principalUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("user_id", _user.Id.ToString()),
                new Claim("email", _user.Email),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principalUser }
            };
        }

        [Fact]
        public async Task Get_GivenNoArgeuments_ReturnCurrentUserAsync()
        {
            //Given
            //When
            _repo.Setup(x => x.GetUser(_user.Id)).ReturnsAsync(_user);

            //Then
            var response = await _controller.Get();
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
            var response = await _controller.UpdateProfile(data);
            var okObjectResult = response as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var okResponse = okObjectResult.Value as OkResponse;
            Assert.Equal("Success", okResponse.Response.ToString());
        }

        [Fact]
        public async Task UpdateProfileImage_GivenImage_ReturnsSuccessAsync()
        {
            //Given
            ImageFromUserDto imageFromUser = Mock.Of<ImageFromUserDto>();
            DataForProfilePicUpdateDto dataForProfileUpdate = Mock.Of<DataForProfilePicUpdateDto>();
            string newProfilePicUrl = "new url";
            //When
            _mapper.Setup(x => x.Map<DataForProfilePicUpdateDto>(imageFromUser)).Returns(dataForProfileUpdate);
            _profileService.Setup(x => x.UploadProfileImage(dataForProfileUpdate)).ReturnsAsync(newProfilePicUrl);
            _repo.Setup(x => x.UpdateProfileImage(_user.Id, newProfilePicUrl)).ReturnsAsync(true);
            //Then
            ActionResult response = await _controller.UpdateProfileImage(imageFromUser);
            OkObjectResult okObjectResult = response as OkObjectResult;
            Assert.NotNull(okObjectResult);
            OkResponse okResponse = okObjectResult.Value as OkResponse;
            Assert.Equal("Success", okResponse.Response.ToString());
        }

        [Fact]
        public async Task View_GivenNothing_ReturnsUserForProfileViewAsyncleViewAsync()
        {
            //Given
            UserForProfileViewDto userForProfileView = Mock.Of<UserForProfileViewDto>();
            userForProfileView.Id = _user.Id;
            userForProfileView.Category = "any";
            string institutionName = "Existing Institution";
            //When
            _repo.Setup(x => x.GetUser(_user.Id)).ReturnsAsync(_user);
            _mapper.Setup(x => x.Map<UserForProfileViewDto>(_user)).Returns(userForProfileView);
            _institution.Setup(x => x.FindName(userForProfileView.Category, _user.InstitutionId)).ReturnsAsync(institutionName);
            //Then
            var response = await _controller.View();
            OkObjectResult okObjectResult = response.Result as OkObjectResult;
            var responseData = okObjectResult.Value as UserForProfileViewDto;
            UserForProfileViewDto userFromController = Assert.IsAssignableFrom<UserForProfileViewDto>(responseData);
            Assert.Equal(_user.Id, userFromController.Id);
            Assert.Equal(institutionName, userFromController.InstitutionName);
        }
    }
}