namespace Infrastructure.Models.ViewModels
{
    public class BookCardVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public double Rating { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public int Stock { get; set; }
        public string PublisherName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
