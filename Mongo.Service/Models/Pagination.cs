namespace Mongo.Domain.Models
{
    public class Pagination
    {
        public int PageSize { get; set; } = 24;

        public int Page { get; set; } = 1;

        public bool Status { get; set; } = true;
    }
}
