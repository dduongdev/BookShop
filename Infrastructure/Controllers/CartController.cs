using Microsoft.AspNetCore.Mvc;
using UseCases;
using System.Linq;
using System.Threading.Tasks;
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

        // Hiển thị giỏ hàng
        //public async Task<IActionResult> Index()
        //{
        //    var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        //    if (userId == null)
        //    {
        //        return BadRequest("User not found.");
        //    }

        //    var cartItems = await _cartItemManager.GetByUserIdAsync(int.Parse(userId));

        //    // Lấy thông tin sách từ CartItem
        //    var cartItemDetails = cartItems.Select(item => new
        //    {
        //        BookId = item.BookId,
        //        BookTitle = _bookManager.GetByIdAsync(item.BookId).Result?.Title,
        //        BookPrice = _bookManager.GetByIdAsync(item.BookId).Result?.Price
        //    });

        //    return View(cartItemDetails);
        //}

        public IActionResult Index()
        {
            // Dữ liệu mô phỏng sản phẩm trong giỏ hàng
            var cartItems = new List<dynamic>
            {
                new { BookId = 1, BookTitle = "C# Programming", BookPrice = 174000.99m },
                new { BookId = 2, BookTitle = "ASP.NET Core MVC", BookPrice = 200000m },
                new { BookId = 3, BookTitle = "Learn Entity Framework", BookPrice = 249000.99m },
                new { BookId = 1, BookTitle = "C# Programming", BookPrice = 174000.99m },
                new { BookId = 2, BookTitle = "ASP.NET Core MVC", BookPrice = 200000m },
                new { BookId = 1, BookTitle = "C# Programming", BookPrice = 174000.99m },
                new { BookId = 2, BookTitle = "ASP.NET Core MVC", BookPrice = 200000m }
            };

            return View(cartItems);  // Truyền dữ liệu vào view
        }



        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public async Task<IActionResult> DeleteItemAsync(int bookId)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest("User not found.");
            }

            var cartItems = await _cartItemManager.GetByUserIdAsync(int.Parse(userId));
            var cartItemToDelete = cartItems.FirstOrDefault(item => item.BookId == bookId);

            if (cartItemToDelete != null)
            {
                await _cartItemManager.DeleteAsync(cartItemToDelete.Id);
            }

            return RedirectToAction("Index");
        }
    }
}
