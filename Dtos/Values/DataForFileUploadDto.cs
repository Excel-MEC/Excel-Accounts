using Microsoft.AspNetCore.Http;

namespace Excel_Accounts_Backend.Dtos.Values
{
    public class DataForFileUploadDto
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
        public string ImageStorageName { get; set; }
    }
}