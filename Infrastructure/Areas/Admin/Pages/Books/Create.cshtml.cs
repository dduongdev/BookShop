using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using UseCases;

namespace Areas.Admin.Pages.Books
{
    public class CreateModel : PageModel
    {
        private readonly BookManager _bookManager;

        public CreateModel(BookManager bookManager)
        {
            _bookManager = bookManager;
        }

        [BindProperty]
        public Book Book { get; set; } // Để bind với form trong view

        // Phương thức GET để hiển thị trang tạo sách
        public IActionResult OnGet()
        {
            return Page(); // Trả về trang Create
        }

        // Phương thức POST để lưu sách vào cơ sở dữ liệu
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Nếu dữ liệu không hợp lệ, quay lại trang Create
            }

            await _bookManager.AddAsync(Book); // Lưu sách mới vào cơ sở dữ liệu
            return RedirectToPage("Index"); // Chuyển về trang danh sách sách
        }
    }
}
