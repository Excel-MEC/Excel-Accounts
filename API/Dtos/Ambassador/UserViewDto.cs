namespace API.Dtos.Ambassador
{
    public class UserViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsPaid { get; set; }
    }
}