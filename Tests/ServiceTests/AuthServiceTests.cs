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

namespace Tests.ServiceTests
{
    public class AuthService2Tests
    {
        private readonly IAuthService2 _AuthService2;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IConfiguration> _config;
        private readonly Mock<IAuthRepository> _repo;
        private readonly HttpClient _httpClient;
        private readonly Mock<IQRCodeGeneration> _qRCodeGeneration;
        private readonly Mock<IAmbassadorRepository> _ambRepo;

        public AuthService2Tests()
        {
            _mapper = new Mock<IMapper>();
            _config = new Mock<IConfiguration>();
            _repo = new Mock<IAuthRepository>();
            _httpClient = new HttpClient();
            _qRCodeGeneration = new Mock<IQRCodeGeneration>();
            _ambRepo = new Mock<IAmbassadorRepository>();
            _AuthService2 = new AuthService2(_mapper.Object, _config.Object, _repo.Object, _httpClient, _qRCodeGeneration.Object, _ambRepo.Object);
        }

        [Fact]
        public async Task CreateJWTForClient_GivenJsonString_ReturnsJWTAsync()
        {
            string email = "a@b.com";
            int id = 1226;
            string tokenSource = "AppSettings:Token";
            string issuerSource = "AppSettings:Issuer";
            string key = "Super Secret Key";
            string issuer = "excelmec.org";
            UserFromAuth0Dto userFromAuth0 = Mock.Of<UserFromAuth0Dto>(x => x.email == email);
            string responseFromAuth0 = JsonSerializer.Serialize(userFromAuth0);
            _repo.Setup(x => x.UserExists(email)).ReturnsAsync(true);
            User user = Mock.Of<User>(x => x.Email == email && x.Id == id && x.Role == Constants.Roles[0]);
            Console.WriteLine("Role is" + user.Role);
            _repo.Setup(x => x.GetUser(email)).ReturnsAsync(user);
            _config.Setup(x => x.GetSection(tokenSource).Value).Returns(key);
            _config.Setup(x => x.GetSection(issuerSource).Value).Returns(issuer);
            var jwt = await _AuthService2.CreateJwtForClient(responseFromAuth0, null);
            var validatedEmail = JwtValidator.Validate(jwt, key, issuer);
            Assert.IsType<string>(jwt);
            Assert.Equal(email, validatedEmail);
        }

        [Fact]
        public async Task FetchUserFromAuth0_GivenInvalidToken_ThrowsUnauthorizedAccessException()
        {
            string access_token = "access_token";
            string auth0Server = "http://ajeshkumar.eu.auth0.com/userinfo";
            string auth0Endpoint = "AppSettings:Auth0Server";
            _config.Setup(x => x.GetSection(auth0Endpoint).Value).Returns(auth0Server);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _AuthService2.FetchUserFromAuth0(access_token));
        }
    }
}