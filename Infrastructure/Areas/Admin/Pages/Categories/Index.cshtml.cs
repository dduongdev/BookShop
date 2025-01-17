using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases;
using UseCases.TaskResults;
using UseCases.UnitOfWork;

namespace Infrastructure.Areas.Admin.Pages.Categories
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly CategoryManager _categoryManager;
        private readonly ICategoryUnitOfWork _categoryUnitOfWork;

        public IndexModel(CategoryManager categoryManager, ICategoryUnitOfWork categoryUnitOfWork)
        {
            _categoryManager = categoryManager;
            _categoryUnitOfWork = categoryUnitOfWork;
        }

        public IEnumerable<Entities.Category> Categories { get; set; } = new List<Entities.Category>();

        public async Task OnGetAsync()
        {
            Categories = await _categoryManager.GetAllAsync();
        }

        public async Task<IActionResult> OnPostSuspendAsync(int id)
        {
            var result = await _categoryManager.SuspendAsync(id);

            if (result.ResultCode == AtomicTaskResultCodes.Success)
            {
                Categories = await _categoryManager.GetAllAsync();
                return Page();
            }

            TempData["ErrorMessage"] = result.Message;
            return Page();
        }

        public async Task<IActionResult> OnPostActivateAsync(int id)
        {
            await _categoryManager.ActivateAsync(id);
            return RedirectToAction("Index");
        }


    }
}
