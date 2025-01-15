using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using UseCases;

namespace Areas.Admin.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly CategoryManager _categoryManager;

        public IndexModel(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        public IEnumerable<Category> Categories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Categories = await _categoryManager.GetAllAsync(); // Lấy tất cả categories
            return Page();
        }

        public async Task<IActionResult> OnPostActivateAsync(int id)
        {
            await _categoryManager.ActivateAsync(id); // Kích hoạt category
            return RedirectToPage(); // Refresh trang
        }

        public async Task<IActionResult> OnPostSuspendAsync(int id)
        {
            await _categoryManager.SuspendAsync(id); // Tạm ngưng category
            return RedirectToPage(); // Refresh trang
        }
    }
}
