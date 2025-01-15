using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using UseCases;

namespace Areas.Admin.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly CategoryManager _categoryManager;

        public EditModel(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [BindProperty]
        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Category = await _categoryManager.GetByIdAsync(id); // Lấy category từ database

            if (Category == null)
            {
                return NotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }

            return Page(); // Nếu tìm thấy, hiển thị trang chỉnh sửa
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Nếu không hợp lệ, giữ nguyên trang Edit
            }

            await _categoryManager.UpdateAsync(Category); // Cập nhật category vào database
            return RedirectToPage("Index"); // Chuyển hướng về trang danh sách categories
        }
    }
}
