using Entities;
using Infrastructure.Models.ViewModels;
using UseCases;

namespace Infrastructure.Services
{
    public class OrderMappingService
    {
        private readonly BookManager _bookManager;
        public async Task<OrderVM> MaptoOrderVM(Order order)
        {
            var orderVM = new OrderVM
            {
                Id = order.Id,
                DeliveryAddress = order.DeliveryAddress,
                DeliveryPhone = order.DeliveryPhone,
                CreatedAt = order.CreatedAt,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                orderItemVMs = new List<OrderItemVM>()
            };

            foreach (var item in order.OrderItems)
            {
                var book = await _bookManager.GetByIdAsync(item.BookId);
                string bookTitle = book?.Title ?? "Unknow Title";

                orderVM.orderItemVMs.Add(new OrderItemVM
                {
                    Id = item.Id,
                    BookTitle = bookTitle,
                    Quantity = item.Quantity
                });
            }
            return orderVM;
        }
    }
}
