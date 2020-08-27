using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using API.Dtos.Auth;
using API.Models;

namespace API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<JwtForClientDto> CreateJwtForClient(string responseInJson, int? referralCode);
        Task<string> FetchUserGoogle0Auth(string access_token);
        Task<string> CreateJwtFromRefreshToken(string token);
        public JwtSecurityToken ValidateToken(string token, byte[] key);
    }
}