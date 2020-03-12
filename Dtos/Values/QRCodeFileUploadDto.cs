using Microsoft.AspNetCore.Http;

namespace Excel_Accounts_Backend.Dtos.Values
{
    public class QRCodeFileUploadDto
    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile Image { get; set; }
        public string ImageStorageName { get; set; }
    }
}