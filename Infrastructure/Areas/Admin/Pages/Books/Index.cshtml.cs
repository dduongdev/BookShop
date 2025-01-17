using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases;

namespace Infrastructure.Areas.Admin.Books
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly BookManager _bookManager;

        public IndexModel(BookManager bookManager)
        {
            _bookManager = bookManager;
        }

        public IEnumerable<Entities.Book> Books { get; set; } = new List<Entities.Book>();

        public async Task OnGetAsync()
        {
            Books = await _bookManager.GetAllAsync();
        }

        public async Task<IActionResult> OnPostSuspendAsync(int id)
        {
            await _bookManager.SuspendAsync(id);
            return RedirectToPage(); // Chuyển hướng về chính trang hiện tại
        }

        public async Task<IActionResult> OnPostActivateAsync(int id)
        {
            await _bookManager.ActivateAsync(id);
            return RedirectToPage(); // Chuyển hướng về chính trang hiện tại
        }
    }
}
