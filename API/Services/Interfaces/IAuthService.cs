using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> CreateJwtForClient(string responseInJson, int? referralCode);
        Task<string> FetchUserGoogle0Auth(string access_token);
    }
}