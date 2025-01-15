using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using UseCases;

namespace Areas.Admin.Pages.Books
{
    public class EditModel : PageModel
    {
        private readonly BookManager _bookManager;

        public EditModel(BookManager bookManager)
        {
            _bookManager = bookManager;
        }

        [BindProperty]
        public Book Book { get; set; } // Để bind với form trong view

        // Phương thức GET để lấy sách theo ID
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var book = await _bookManager.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound(); // Trả về 404 nếu sách không tìm thấy
            }

            Book = book;
            return Page(); // Trả về trang Edit
        }

        // Phương thức POST để lưu thông tin sách
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Nếu dữ liệu không hợp lệ, quay lại trang Edit
            }

            await _bookManager.UpdateAsync(Book); // Cập nhật thông tin sách
            return RedirectToPage("Index"); // Chuyển về trang danh sách sách
        }
    }
}
