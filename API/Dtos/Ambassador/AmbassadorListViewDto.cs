namespace API.Dtos.Ambassador
{
    public class AmbassadorListViewDto
    {
        public int AmbassadorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int FreeMembership { get; set; }
        public int PaidMembership { get; set; }
    }
}