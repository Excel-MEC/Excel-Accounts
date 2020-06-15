namespace API.Dtos.Auth
{
    public class GoogleUserData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string email { get; set; }
        public string picture { get; set; }
    }
}