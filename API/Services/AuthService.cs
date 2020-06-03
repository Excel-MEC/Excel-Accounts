using System;
using System.IdentityModel.Tokens.Jwt;
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
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<string> CreateJwtForClient(GoogleJsonWebSignature.Payload payload, int? referralCode)
        {
            var userFromGoogle0Auth = _mapper.Map<User>(payload);
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
            var claims = new[] {
                new Claim("user_id", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(365),
                SigningCredentials = creds,
                Issuer = _config.GetSection("AppSettings:Issuer").Value 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}