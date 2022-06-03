using System.Collections.Generic;

namespace Mongo.Domain.Models
{
    public class PaginationEntity<TDocument> : Pagination
    {
        public int TotalPages { get; set; }

        public int TotalRows { get; set; }

        public List<TDocument> Data { get; set; }
    }
}
