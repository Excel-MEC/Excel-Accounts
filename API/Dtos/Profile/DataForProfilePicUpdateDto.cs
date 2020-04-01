using Microsoft.AspNetCore.Http;

namespace API.Dtos.Profile
{
    public class DataForProfilePicUpdateDto
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
    }
}