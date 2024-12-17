namespace Infrastructure.Models
{
    public class PaginationMetadata
    {
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPageIndex { get; set; }
    }
}
