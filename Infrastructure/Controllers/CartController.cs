using Microsoft.AspNetCore.Mvc;
using UseCases;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Entities.Enums;

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

        //    var books = await _bookManager.GetAllAsync();
        //    var bookLookup = books.ToDictionary(_ => _.Id);

        //    // Lấy thông tin sách từ CartItem, bao gồm trạng thái và thông báo nếu Suspended
        //    var cartItemDetails = cartItems.Select(item => new
        //    {
        //        BookId = item.BookId,
        //        BookTitle = bookLookup[item.BookId].Title,
        //        BookPrice = bookLookup[item.BookId].Price,
        //        IsSuspended = bookLookup[item.BookId].Status == EntityStatus.Suspended,
        //        SuspendedMessage = bookLookup[item.BookId].Status == EntityStatus.Suspended ? "Sản phẩm hiện đang bị tạm ngưng và không thể mua." : null
        //    });

        //    return View(cartItemDetails);
        //}

        public IActionResult Index()
        {
            // Dữ liệu mô phỏng sản phẩm trong giỏ hàng
            var cartItems = new List<dynamic>
             {
                 new { BookId = 1, BookTitle = "C# Programming", BookPrice = 174000.99m, IsSuspended = false, SuspendedMessage = (string?)null },
                 new { BookId = 2, BookTitle = "ASP.NET Core MVC", BookPrice = 200000m, IsSuspended = true, SuspendedMessage = "Sản phẩm hiện đang bị tạm ngưng và không thể mua." },
                 new { BookId = 3, BookTitle = "Learn Entity Framework", BookPrice = 249000.99m, IsSuspended = false, SuspendedMessage = (string?)null },
                 new { BookId = 4, BookTitle = "Mastering LINQ", BookPrice = 180000m, IsSuspended = true, SuspendedMessage = "Sản phẩm hiện đang bị tạm ngưng và không thể mua." }
             };

            return View(cartItems);  // Truyền dữ liệu vào view
        }




        [HttpPost]
        public async Task<IActionResult> AddAsync(int? bookId)
        {
            if (bookId == null)
            {
                return BadRequest();
            }

            var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                await _cartItemManager.AddAsync(new Entities.CartItem { BookId = bookId.Value, UserId = userId });
            }

            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            await _cartItemManager.DeleteAsync(id.Value);

            return RedirectToAction("Index");
        }
    }
}
