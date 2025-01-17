using Entities.Enums;

namespace Infrastructure.Models.ViewModels
{
    public class CartItemVM
    {
        public int Id { get; set; }
        public int BookId {  get; set; }
        public required string BookTitle { get; set; }
        public EntityStatus Status { get; set; }
        public int BookPrice {  get; set; }
        public string BookName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
