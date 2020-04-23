using System.Collections.Generic;

namespace API.Models
{
    public class Ambassador
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<User> ReferredUsers { get; set; }
        public int FreeMembership { get; set; }
        public int PaidMembership { get; set; }
    }
}