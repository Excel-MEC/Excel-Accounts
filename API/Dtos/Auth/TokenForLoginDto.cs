namespace API.Dtos.Auth
{
    public class TokenForLoginDto
    {
        public string accessToken { get; set; }
        public int? referralCode { get; set; }
        
    }
}