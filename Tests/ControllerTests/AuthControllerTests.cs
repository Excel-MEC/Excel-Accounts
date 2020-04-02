using System;
using System.Threading.Tasks;
using API.Controllers;
using API.Dtos.Auth;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.ControllerTests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authService;
        private readonly AuthController _controller;
        public AuthControllerTests()
        {
            _authService = new Mock<IAuthService>();
            _controller = new AuthController(_authService.Object);
        }

        [Fact]
        public async Task Login_GivenAccessToken_ReturnsJWTAsync()
        {
            string response = "response";
            _authService.Setup(x => x.FetchUserFromAuth0(It.IsAny<string>())).ReturnsAsync(response);
            _authService.Setup(x => x.CreateJwtForClient(It.IsAny<string>())).ReturnsAsync(response);
            var tokenForLogin = Mock.Of<TokenForLoginDto>();
            var result = await _controller.Login(tokenForLogin);
            Assert.IsType<ActionResult<JwtForClientDto>>(result);
        }
    }
}