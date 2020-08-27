using API.Models.Custom;

namespace API.Dtos.Admin
{
    public class QueryParametersForGetAllUsers : PaginationParameters
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int? InstitutionId { get; set; }
        
        public string MobileNumber { get; set; }
        public int? CategoryId { get; set; }
        public int? ReferrerAmbassadorId { get; set; }
        public bool? IsPaid { get; set; }
        public string SortOrder { get; set; }
    }
}