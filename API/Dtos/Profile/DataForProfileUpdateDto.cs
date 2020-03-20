namespace API.Dtos.Profile
{
    public class DataForProfileUpdateDto
    {
        public int InstitutionId { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public string InstitutionName { get; set; }
        public bool IsCollege { get; set; }
    }
}