using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using UseCases;

namespace Areas.Admin.Pages.Publishers
{
    public class CreateModel : PageModel
    {
        private readonly PublisherManager _publisherManager;

        public CreateModel(PublisherManager publisherManager)
        {
            _publisherManager = publisherManager;
        }

        [BindProperty]
        public Publisher Publisher { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // N?u form không h?p l?, quay l?i trang Create
            }

            await _publisherManager.AddAsync(Publisher); // Thêm m?i Publisher
            return RedirectToPage("Index"); // Chuy?n h??ng v? trang Index
        }
    }
}
