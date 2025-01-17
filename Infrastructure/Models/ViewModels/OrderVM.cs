using Entities.Enums;

namespace Infrastructure.Models.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public string Status { get; set; } = String.Empty;
        public PaymentMethod PaymentMethod {  get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public required string DeliveryAddress { get; set; }
        public required string DeliveryPhone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItemVM> orderItemVMs { get; set; } = new List<OrderItemVM>();
    }
}
