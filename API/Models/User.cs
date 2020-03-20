using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Picture { get; set; }
        public string QRCodeUrl { get; set; }
        public int InstitutionId { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public bool IsCollege { get; set; }
    }
}