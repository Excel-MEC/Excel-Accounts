using System.ComponentModel.DataAnnotations;

namespace Excel_Accounts_Backend.Models
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
    }
}