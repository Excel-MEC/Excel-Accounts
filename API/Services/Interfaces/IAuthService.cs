using System.Threading.Tasks;
using Google.Apis.Auth;

namespace API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> CreateJwtForClient(GoogleJsonWebSignature.Payload payload, int? referralCode);
    }
}