using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Models;
using API.Models.Custom;
using API.Services.Interfaces;
using AutoMapper;
using Google.Apis.Auth;
using API.Dtos.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IAuthRepository _repo;
        private readonly HttpClient _httpClient;
        private readonly IQRCodeGeneration _qRCodeGeneration;
        private readonly IAmbassadorRepository _ambRepo;

        public AuthService(IMapper mapper, IConfiguration config, IAuthRepository repo, HttpClient httpClient, IQRCodeGeneration qRCodeGeneration, IAmbassadorRepository ambRepo)
        {
            _ambRepo = ambRepo;
            _qRCodeGeneration = qRCodeGeneration;
            _mapper = mapper;
            _config = config;
            _repo = repo;
            _httpClient = httpClient;
        }

        public async Task<string> CreateJwtForClient(string responseString, int? referralCode)
        {
            var userFrom0Auth = JsonConvert.DeserializeObject<UserFromAuth0Dto>(responseString);   
            var userFromGoogle0Auth = _mapper.Map<User>(userFrom0Auth);
            if (!await _repo.UserExists(userFromGoogle0Auth.Email))
            {
                var newUser = userFromGoogle0Auth;
                newUser.QRCodeUrl = await _qRCodeGeneration.CreateQrCode(newUser.Id.ToString());
                newUser.Role = Constants.Roles[0];
                newUser = await _repo.Register(newUser);
                int referral = referralCode ?? default(int);
                if (referralCode != null)
                {

                    await _ambRepo.ApplyReferralCode(newUser.Id, referral);
                }
            }
            User user = await _repo.GetUser(userFromGoogle0Auth.Email);
            var claims = new List<Claim>() {
                new Claim("user_id", user.Id.ToString()),
                new Claim("email", user.Email)
            };
            foreach (var role in user.Role.Split(",").Select(x => x.Trim()))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TOKEN")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(365),
                SigningCredentials = creds,
                Issuer = Environment.GetEnvironmentVariable("ISSUER")
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> FetchUserGoogle0Auth(string accessToken)
        {
            string googleapi = Environment.GetEnvironmentVariable("GOOGLEAPI");
            Console.WriteLine("vdgsvdgvg:"+googleapi);
            var url = new Uri(googleapi);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync(url);
           
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Authorization header has been set, but the server reports that it is missing.
                // It was probably stripped out due to a redirect.

                var finalRequestUri = response.RequestMessage.RequestUri; // contains the final location after following the redirect.                

                if (finalRequestUri != url) // detect that a redirect actually did occur.
                {
                    // If this is public facing, add tests here to determine if Url should be trusted
                    response = await _httpClient.GetAsync(finalRequestUri);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                }    
            }   
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}