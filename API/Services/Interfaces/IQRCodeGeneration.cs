using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IQRCodeGeneration
    {
        Task<string> CreateQrCode(string id);
    }
}