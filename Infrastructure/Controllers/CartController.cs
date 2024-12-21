using Microsoft.AspNetCore.Mvc;
using Entities;
using UseCases;
using Infrastructure.Models.ViewModels;
using System.Security.Claims;


namespace Infrastructure.Controllers
{
    public class CartController : Controller
    {
        private readonly CartItemManager _cartItemManager;
        private readonly BookManager _bookManager;

        public CartController(CartItemManager cartItemManager, BookManager bookManager)
        {
            _cartItemManager = cartItemManager;
            _bookManager = bookManager;
        }

        // Hiển thị danh sách giỏ hàng
        //public async Task<IActionResult> Index()
        //{
        //    // Lấy danh sách CartItem từ database
        //    var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
        //    {
        //        var cartItems = await _cartItemManager.GetByUserIdAsync(userId);
        //        var viewModel = cartItems.Select(async cartItem =>
        //        {
        //            var book = await _bookManager.GetByIdAsync(cartItem.BookId); // Lấy thông tin Book
        //            return new CartItemViewModel
        //            {
        //                Id = cartItem.Id,
        //                BookName = book?.Title ?? "Không xác định", // Xử lý nếu book không tồn tại
        //                Quantity = cartItem.Quantity
        //            };
        //        });

        //        return View(await Task.WhenAll(viewModel));
        //    }

        //    return BadRequest();
        //}

        public async Task<IActionResult> Index()
        {
            // Giả lập dữ liệu CartItem
            var cartItems = new List<CartItem>
    {
        new CartItem { Id = 1, BookId = 101, Quantity = 2 },
        new CartItem { Id = 2, BookId = 102, Quantity = 1 },
        new CartItem { Id = 3, BookId = 103, Quantity = 3 }
    };

            // Giả lập dữ liệu Book để truyền vào CartItemViewModel
            var books = new Dictionary<int, string>
    {
        { 101, "Book Title 1" },
        { 102, "Book Title 2" },
        { 103, "Book Title 3" }
    };

            // Chuyển đổi CartItem thành CartItemViewModel
            var viewModel = cartItems.Select(cartItem =>
            {
                var bookName = books.ContainsKey(cartItem.BookId) ? books[cartItem.BookId] : "Không xác định";
                return new CartItemViewModel
                {
                    Id = cartItem.Id,
                    BookName = bookName,
                    Quantity = cartItem.Quantity
                };
            }).ToList();

            return View(viewModel);
        }

        // Thêm sản phẩm vào giỏ hàng (bằng cách gọi vào CartItemManager)
        [HttpPost]
        public async Task<IActionResult> Add(int bookId)
        {
            var book = await _bookManager.GetByIdAsync(bookId);
            if (book == null)
            {
                return NotFound("Không tìm thấy sách.");
            }

            // Kiểm tra nếu sản phẩm đã tồn tại trong giỏ hàng
            var existingCartItem = (await _cartItemManager.GetAllAsync())
                .FirstOrDefault(ci => ci.BookId == bookId);

            if (existingCartItem != null)
            {
                // Tăng số lượng sản phẩm nếu đã tồn tại
                existingCartItem.Quantity++;
                await _cartItemManager.UpdateAsync(existingCartItem);
            }
            else
            {
                // Tạo mới nếu chưa tồn tại
                var newCartItem = new CartItem
                {
                    BookId = bookId,
                    Quantity = 1
                };
                await _cartItemManager.AddAsync(newCartItem);
            }

            return RedirectToAction(nameof(Index)); // Redirect về trang giỏ hàng sau khi thêm sản phẩm
        }

        // Cập nhật số lượng CartItem
        [HttpPost]
        public async Task<IActionResult> Update(int id, int quantity)
        {
            var cartItem = await _cartItemManager.GetByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            // Cập nhật số lượng sản phẩm
            cartItem.Quantity = quantity;
            await _cartItemManager.UpdateAsync(cartItem);

            return RedirectToAction(nameof(Index)); // Redirect về trang giỏ hàng sau khi cập nhật
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _cartItemManager.DeleteAsync(id);
            return RedirectToAction(nameof(Index)); // Redirect về trang giỏ hàng sau khi xóa sản phẩm
        }
    }

}
