namespace Stephs_Shop.Areas.Admin.Models
{
    public class PaginatedResult<T> 
    {
        public int PageCount { get; set; }
        public T[] Items { get; set; }
        public int Page { get; set; }

    }
}
