using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases;

namespace Infrastructure.Areas.Admin.Pages.Categories
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly CategoryManager _categoryManager;

        public EditModel(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [BindProperty]
        public Entities.Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Tìm danh mục theo ID
            Category = await _categoryManager.GetByIdAsync(id);

            if (Category == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _categoryManager.UpdateAsync(Category);

            return RedirectToPage("./Index");
        }
    }
}
