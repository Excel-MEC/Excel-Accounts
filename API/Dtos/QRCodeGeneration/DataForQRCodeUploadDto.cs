using Microsoft.AspNetCore.Http;

namespace API.Dtos.QRCodeGeneration
{
    public class DataForQRCodeUploadDto
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }  
    }
}