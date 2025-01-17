using Entities;
using Infrastructure.Models.ViewModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UseCases;

namespace Infrastructure.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager _userManager;
        private readonly BookManager _bookManager;
        private readonly OrderManager _orderManager;
        private readonly BookMappingService _bookMappingService;

        public OrderController(UserManager userManager, BookManager bookManager, BookMappingService bookMappingService, OrderManager orderManager)
        {
            _userManager = userManager;
            _bookManager = bookManager;
            _bookMappingService = bookMappingService;
            _orderManager = orderManager;
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAsync(IEnumerable<int> orderedBookIds)
        {
            if (orderedBookIds == null || !orderedBookIds.Any())
            {
                return BadRequest();
            }

            List<Book> orderedBooks = new List<Book>();
            foreach (var id in orderedBookIds)
            {
                var foundBook = await _bookManager.GetByIdAsync(id);
                if (foundBook == null)
                {
                    return BadRequest();
                }

                orderedBooks.Add(foundBook);
            }

            IEnumerable<BookCardVM> bookCardVMs = await _bookMappingService.MapToBookCardVMs(orderedBooks);

            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest();
            }

            var user = await _userManager.GetByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            ViewBag.DefaultDeliveryAddress = user.Address;
            ViewBag.DefaultDeliveryPhone = user.Phone;

            return View(bookCardVMs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Order order, double totalAmount)
        {
            if (order == null || !order.OrderItems.Any())
            {
                return BadRequest();
            }

            if (order.PaymentMethod != Entities.Enums.PaymentMethod.COD)
            {
                order.PaymentStatus = Entities.Enums.PaymentStatus.Pending;
            }

            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                order.UserId = userId;
            }
            else
            {
                return BadRequest();
            }

            var createOrderResult = await _orderManager.CreateAsync(order);
            if (createOrderResult.ResultCode == UseCases.TaskResults.AtomicTaskResultCodes.Failed)
            {
                return StatusCode(500, "An internal error occurred while creating the order.");
            }

            if (order.PaymentMethod == Entities.Enums.PaymentMethod.VNPay)
            {
                return RedirectToAction("RedirectToPaymentUrl", "VNPay", new { OrderId = order.Id, Money = totalAmount, Description = $"THANH TOAN CHO DON HANG {order.Id}" });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
