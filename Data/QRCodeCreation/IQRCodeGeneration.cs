using System.Threading.Tasks;
namespace Excel_Accounts_Backend.Data.QRCodeCreation
{
    public interface IQRCodeGeneration
    {
        Task<string> CreateQrCode(string ExcelId);     
    }
}