using Entities.Enums;

namespace Infrastructure.Models.ViewModels
{
    public class BookDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int Stock { get; set; }
        public string PublisherName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public EntityStatus Status { get; set; }
        public IEnumerable<string> Images = new List<string>();
    }
}
