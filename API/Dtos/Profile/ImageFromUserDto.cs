using Microsoft.AspNetCore.Http;

namespace API.Dtos.Profile
{
    public class ImageFromUserDto
    {
        public IFormFile Image { get; set; }
    }
}