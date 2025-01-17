using Entities;
using Infrastructure.Models.ViewModels;
using UseCases;

namespace Infrastructure.Services
{
    public class OrderMappingService
    {
        private readonly BookManager _bookManager;

        public OrderMappingService(BookManager bookManager)
        {
            _bookManager = bookManager;
        }

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

            var books = await _bookManager.GetAllAsync();

            //var bookIds = order.OrderItems.Select(item => item.BookId).Distinct().ToList();

            var bookDictionary = books.ToDictionary(book => book.Id);

            foreach (var item in order.OrderItems)
            {
                var book = bookDictionary.GetValueOrDefault(item.BookId);
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
