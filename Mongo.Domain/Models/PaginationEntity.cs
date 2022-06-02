using System.Collections.Generic;

namespace Mongo.Domain.Models
{
    public class PaginationEntity<TDocument> : Pagination
    {
        public int totalPages { get; set; }
        public int totalRows { get; set; }
        public List<TDocument> data { get; set; }
    }
}
