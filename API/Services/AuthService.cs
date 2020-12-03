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
        private readonly IEnvironmentService _env;

        public AuthService(IMapper mapper, IConfiguration config, IAuthRepository repo, HttpClient httpClient,
            IQRCodeGeneration qRCodeGeneration, IAmbassadorRepository ambRepo, IEnvironmentService env)
        {
            _ambRepo = ambRepo;
            _qRCodeGeneration = qRCodeGeneration;
            _mapper = mapper;
            _config = config;
            _repo = repo;
            _httpClient = httpClient;
            _env = env;
        }

        public async Task<JwtForClientDto> CreateJwtForClient(string responseString, int? referralCode)
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
            var jwtForClient = new JwtForClientDto();
            var jwtKey = Encoding.ASCII.GetBytes(_env.AccessToken);
            jwtForClient.AccessToken = CreateAccessTokenFromUser(user, jwtKey);
            var claims = new List<Claim>()
            {
                new Claim("user_id", user.Id.ToString()),
                new Claim("email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_env.RefreshToken));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddYears(1),
                SigningCredentials = creds,
                Issuer = _env.Issuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            jwtForClient.RefreshToken = tokenHandler.WriteToken(token);
            return jwtForClient;
        }

        public async Task<string> FetchUserGoogle0Auth(string accessToken)
        {
            string googleApi = _env.GoogleApi;
            var url = new Uri(googleApi);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var finalRequestUri =
                    response.RequestMessage
                        .RequestUri;
                if (finalRequestUri != url)
                {
                    response = await _httpClient.GetAsync(finalRequestUri);
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new UnauthorizedAccessException();
                }
                else
                    throw new UnauthorizedAccessException();
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> CreateJwtFromRefreshToken(string token)
        {
            var refreshKey = Encoding.ASCII.GetBytes(_env.RefreshToken);
            var securityToken = ValidateToken(token, refreshKey);
            var userId = int.Parse(securityToken.Claims.First(i => i.Type == "user_id").Value);
            var user = await _repo.GetUserById(userId);
            if(user == null)
                throw new UnauthorizedAccessException();
            var accessKey = Encoding.ASCII.GetBytes(_env.AccessToken);
            return CreateAccessTokenFromUser(user, accessKey);
        }

        public JwtSecurityToken ValidateToken(string token, byte[] key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _env.Issuer,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return (JwtSecurityToken) validatedToken;
        }

        private string CreateAccessTokenFromUser(User user, byte[] refreshKey)
        {
            try
            {
                var claims = new List<Claim>()
                {
                    new Claim("user_id", user.Id.ToString()),
                    new Claim("name", user.Name),
                    new Claim("email", user.Email),
                    new Claim("isPaid", user.IsPaid.ToString()),
                };
                foreach (var role in user.Role.Split(",").Select(x => x.Trim()))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(refreshKey);

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddMinutes(15),
                    SigningCredentials = creds,
                    Issuer = _env.Issuer
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var newToken = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(newToken);
            }
            catch
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}