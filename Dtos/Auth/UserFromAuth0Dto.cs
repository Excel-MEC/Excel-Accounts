using System.ComponentModel.DataAnnotations;

namespace Excel_Accounts_Backend.Dtos.Auth
{
    public class UserFromAuth0Dto
    {
        public string sub { get; set; }
        [Required]
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string nickname { get; set; }
        public string name { get; set; }
        [Required]
        public string picture { get; set; }
        public string locale { get; set; }
        public string updated_at { get; set; }
        [Required]
        public string email { get; set; }
        public bool email_verified { get; set; }
    }
}