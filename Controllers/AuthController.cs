using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Excel_Accounts_Backend.Data.AuthRepository;
using Excel_Accounts_Backend.Dtos.Auth;
using Excel_Accounts_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Excel_Accounts_Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IAuthRepository _repo;

        public AuthController(IMapper mapper, IAuthRepository repo, IConfiguration config)
        {
            _mapper = mapper;
            _repo = repo;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(TokenForLoginDto tokenForLogin)
        {
            var httpClient = new HttpClient();
            var url = new Uri("http://ajeshkumar.eu.auth0.com/userinfo");
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenForLogin.auth_token);
            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Authorization header has been set, but the server reports that it is missing.
                // It was probably stripped out due to a redirect.

                var finalRequestUri =
                    response.RequestMessage.RequestUri; // contains the final location after following the redirect.

                if (finalRequestUri != url) // detect that a redirect actually did occur.
                {
                    // If this is public facing, add tests here to determine if Url should be trusted
                    response = await httpClient.GetAsync(finalRequestUri);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return Unauthorized();
                    }
                }
            }

            var responseInJson = response.Content.ReadAsStringAsync().Result;
            // var decoded = JObject.Parse(response_in_json);
            var userFromAuth0 = JsonConvert.DeserializeObject<UserFromAuth0Dto>(responseInJson);
            if (!await _repo.UserExists(userFromAuth0.email))
            {
                var newUser = _mapper.Map<User>(userFromAuth0);
                await _repo.Register(newUser);
            }
            User user = await _repo.GetUser(userFromAuth0.email);
            var claims = new[] {
                new Claim("Id", user.Id.ToString()),
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
                new Claim("Picture", user.Picture)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}