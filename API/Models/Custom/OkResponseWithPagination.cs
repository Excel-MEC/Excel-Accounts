using System.Collections.Generic;

namespace API.Models.Custom
{
    public class OkResponseWithPagination<T>
    {
        public List<T> Data { get; set; }
        public Pagination Pagination { get; set; }
    }
}