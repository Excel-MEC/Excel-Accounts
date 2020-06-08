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
        private readonly Mock<IAuthService2> _AuthService2;
        private readonly Mock<IAuthService> _AuthService;
        private readonly AuthController _controller;
        public AuthControllerTests()
        {
            _AuthService2 = new Mock<IAuthService2>();
            _AuthService = new Mock<IAuthService>();
            _controller = new AuthController(_AuthService.Object,_AuthService2.Object);
        }

        [Fact]
        public async Task Login_GivenAccessToken_ReturnsJWTAsync()
        {
            string response = "response";
            _AuthService2.Setup(x => x.FetchUserFromAuth0(It.IsAny<string>())).ReturnsAsync(response);
            _AuthService2.Setup(x => x.CreateJwtForClient(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(response);
            var tokenForLogin = Mock.Of<TokenForLogin2Dto>();
            var result = await _controller.Login2(tokenForLogin);
            Assert.IsType<ActionResult<JwtForClientDto>>(result);
        }
    }
}