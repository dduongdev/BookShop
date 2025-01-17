using Entities;
using Infrastructure.Models;
using Infrastructure.Models.ViewModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases;

namespace Infrastructure.Areas.Admin.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly OrderManager _orderManager;
        private readonly OrderMappingService _orderMappingService;

        public IndexModel(OrderManager orderManager, OrderMappingService orderMappingService)
        {
            _orderManager = orderManager;
            _orderMappingService = orderMappingService;
        }

        [BindProperty]
        public List<OrderVM> OrderVMs { get; set; } = default!;
        public PaginationMetadata PaginationMetadata { get; set; }

        public async Task<IActionResult> OnGet(int pageIndex = 1)
        {
            var orders = await _orderManager.GetAllAsync();
            PaginationService<Order> paginationService = new PaginationService<Order>(orders, 12);
            var paginatedOrders = paginationService.GetItemsByPage(pageIndex);
            paginatedOrders = paginatedOrders.OrderByDescending(o => o.CreatedAt);

            OrderVMs = new List<OrderVM>();
            foreach (var order in paginatedOrders)
            {
                var orderVM = await _orderMappingService.MaptoOrderVM(order);
                OrderVMs.Add(orderVM);
            }

            PaginationMetadata = paginationService.GetPaginationMetadata();

            return Page();
        }
    }
}
