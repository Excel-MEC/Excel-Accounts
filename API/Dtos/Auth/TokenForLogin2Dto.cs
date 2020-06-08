namespace API.Dtos.Auth
{
    public class TokenForLogin2Dto
    {
        public string auth_token { get; set; }
        public int? referralCode { get; set; }
    }
}