namespace API.Dtos.Profile
{
    public class DataForProfileViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public string QRCodeUrl { get; set; }
        public string InstitutionName { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public string Category{ get; set; }
    }
}