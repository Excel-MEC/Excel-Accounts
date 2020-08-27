namespace API.Dtos.Auth
{
    public class JwtForClientDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}