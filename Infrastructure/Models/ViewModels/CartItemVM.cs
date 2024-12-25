namespace Infrastructure.ViewModels
{
    public class CartItemVM
    {
        public int Id { get; set; }
        public string BookName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
