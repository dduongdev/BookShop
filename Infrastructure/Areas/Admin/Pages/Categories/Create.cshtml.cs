using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using UseCases;

namespace Areas.Admin.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly CategoryManager _categoryManager;

        public CreateModel(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [BindProperty]
        public Category Category { get; set; }

        public IActionResult OnGet()
        {
            return Page(); // Hiển thị trang Create
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Nếu không hợp lệ, giữ nguyên trang Create
            }

            await _categoryManager.AddAsync(Category); // Thêm category vào cơ sở dữ liệu
            return RedirectToPage("Index"); // Chuyển hướng về trang danh sách categories
        }
    }
}
