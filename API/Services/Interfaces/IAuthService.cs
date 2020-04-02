using System.Net.Http;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> FetchUserFromAuth0(string access_token);
        Task<string> CreateJwtForClient(string responseString);
    }
}