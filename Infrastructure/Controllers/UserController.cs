using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Security.Claims;
using UseCases;
using Entities;
using Infrastructure.Services;
using Infrastructure.Models.ViewModels;

namespace Infrastructure.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager _userManager;

        private readonly OrderManager _orderManager;

        private readonly OrderMappingService _orderMappingService;

        public UserController(UserManager userManager, OrderManager orderManager, OrderMappingService orderMappingService)
        {
            _userManager = userManager;
            _orderManager = orderManager;
            _orderMappingService = orderMappingService;
        }
        
        public async Task<IActionResult> Index()
        {
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if(userIdClaim == null)
            {
                return NotFound("Invalid User");
            }

            if(!int.TryParse(userIdClaim , out var userId))
            {
                return BadRequest();
            }

            //Get user information
            var user = await _userManager.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            //Get orders
            var orders = await _orderManager.GetByIdAsync(userId);
            if(orders == null)
            {
                return NotFound("No Orders");
            }

            var ordersList = await _orderManager.GetAllAsync();
            var userOrders = ordersList.Where(o => o.UserId == userId).ToList();

            var orderVMs = new List<OrderVM>();
            foreach(var order in userOrders)
            {
                var orderVM = await _orderMappingService.MaptoOrderVM(order);
                orderVMs.Add(orderVM);
            }

            ViewData["User"] = user;
            ViewData["Orders"] = orderVMs;

            return View();
        }
        
    }
}
