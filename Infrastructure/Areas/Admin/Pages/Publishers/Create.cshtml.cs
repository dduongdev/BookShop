using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases;

namespace Infrastructure.Areas.Admin.Pages.Publishers
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly PublisherManager _publisherManager;

        public CreateModel(PublisherManager publisherManager)
        {
            _publisherManager = publisherManager;
        }

        [BindProperty]
        public Entities.Publisher Publisher { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _publisherManager.AddAsync(Publisher);
            return RedirectToPage("Index");
        }
    }
}
