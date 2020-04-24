namespace API.Dtos.Auth
{
    public class TokenForLoginDto
    {
        public string auth_token { get; set; }
        public int? referralCode { get; set; }
    }
}