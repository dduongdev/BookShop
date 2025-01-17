using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases;

namespace Infrastructure.Areas.Admin.Pages.Publishers
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly PublisherManager _publisherManager;

        public EditModel(PublisherManager publisherManager)
        {
            _publisherManager = publisherManager;
        }

        [BindProperty]
        public Entities.Publisher Publisher { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Publisher = await _publisherManager.GetByIdAsync(id);

            if (Publisher == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _publisherManager.UpdateAsync(Publisher);
            return RedirectToPage("Index");
        }
    }
}
