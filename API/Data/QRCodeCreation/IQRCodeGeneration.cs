using System.Threading.Tasks;
namespace API.Data.QRCodeCreation
{
    public interface IQRCodeGeneration
    {
        Task<string> CreateQrCode(string ExcelId);     
    }
}