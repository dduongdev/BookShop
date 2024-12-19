using Microsoft.AspNetCore.Mvc;
using Entities;
using UseCases;

namespace Infrastructure.Controllers
{
    public class CartController : Controller
    {
        private readonly CartItemManager _cartItemManager;

        public CartController(CartItemManager cartItemManager)
        {
            _cartItemManager = cartItemManager;
        }

        // Danh sách các CartItem
        public async Task<IActionResult> Index()
        {
            var cartItems = await _cartItemManager.GetAllAsync();
            return View(cartItems);
        }

        // Hiển thị form thêm mới CartItem
        public IActionResult Add()
        {
            return View();
        }

        // Thêm mới CartItem
        [HttpPost]
        public async Task<IActionResult> Add(CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                await _cartItemManager.AddAsync(cartItem);
                return RedirectToAction(nameof(Index)); // Quay lại danh sách sau khi thêm.
            }
            return View(cartItem);
        }

        // Hiển thị form cập nhật CartItem
        public async Task<IActionResult> Update(int id)
        {
            var cartItem = await _cartItemManager.GetByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            return View(cartItem);
        }

        // Cập nhật CartItem
        [HttpPost]
        public async Task<IActionResult> Update(CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                await _cartItemManager.UpdateAsync(cartItem);
                return RedirectToAction(nameof(Index));
            }
            return View(cartItem);
        }

        // Xóa CartItem
        public async Task<IActionResult> Delete(int id)
        {
            await _cartItemManager.DeleteAsync(id);
            return RedirectToAction(nameof(Index)); 
        }
    }
}
