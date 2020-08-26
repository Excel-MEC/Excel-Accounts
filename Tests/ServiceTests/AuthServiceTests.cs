using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Auth;
using API.Models;
using API.Models.Custom;
using API.Services;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ServiceTests
{
    public class AuthServiceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IAuthService _authService;
        private readonly Mock<IConfiguration> _config;
        private readonly Mock<IAuthRepository> _repo;

        public AuthServiceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var mapper = new Mock<IMapper>();
            _config = new Mock<IConfiguration>();
            _repo = new Mock<IAuthRepository>();
            var httpClient = new HttpClient();
            var qRCodeGeneration = new Mock<IQRCodeGeneration>();
            var ambRepo = new Mock<IAmbassadorRepository>();
            _authService = new AuthService(mapper.Object, _config.Object, _repo.Object, httpClient, qRCodeGeneration.Object, ambRepo.Object);
        }

        [Fact]
        public async Task CreateJWTForClient_GivenJsonString_ReturnsJWTAsync()
        {
            const string email = "a@b.com";
            const int id = 1226;
            var tokenSource = Environment.GetEnvironmentVariable("TOKEN");
            var issuerSource = Environment.GetEnvironmentVariable("ISSUER");
            var key = Environment.GetEnvironmentVariable("SECRET_KEY");
            const string issuer = "excelmec.org";
            var userFromAuth0 = Mock.Of<UserFromAuth0Dto>(x => x.email == email);
            var responseFromAuth0 = JsonSerializer.Serialize(userFromAuth0);
            _repo.Setup(x => x.UserExists(email)).ReturnsAsync(true);
            var user = Mock.Of<User>(x => x.Email == email && x.Id == id && x.Role == Constants.Roles[0]);
            _testOutputHelper.WriteLine("Role is" + user.Role);
            _repo.Setup(x => x.GetUser(email)).ReturnsAsync(user);
            _config.Setup(x => x.GetSection(tokenSource).Value).Returns(key);
            _config.Setup(x => x.GetSection(issuerSource).Value).Returns(issuer);
            var jwt = await _authService.CreateJwtForClient(responseFromAuth0, null);
            var validatedEmail = JwtValidator.Validate(jwt, key, issuer);
            Assert.IsType<string>(jwt);
            Assert.Equal(email, validatedEmail);
        }

        [Fact]
        public async Task FetchUserFromAuth0_GivenInvalidToken_ThrowsUnauthorizedAccessException()
        {
            const string accessToken = "access_token";
            const string googleOAuthServer = "https://www.googleapis.com/oauth2/v1/userinfo";
            var googleOAuthEndpoint = Environment.GetEnvironmentVariable("GOOGLEAPI");
            _config.Setup(x => x.GetSection(googleOAuthEndpoint).Value).Returns(googleOAuthServer);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.FetchUserGoogle0Auth(accessToken));
        }
    }
}