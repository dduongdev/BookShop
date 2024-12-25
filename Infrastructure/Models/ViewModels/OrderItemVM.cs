namespace Infrastructure.Models.ViewModels
{
    public class OrderItemVM
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public required int Quantity { get; set; }
    }
}
