using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using UseCases;

namespace Areas.Admin.Pages.Publishers
{
    public class EditModel : PageModel
    {
        private readonly PublisherManager _publisherManager;

        public EditModel(PublisherManager publisherManager)
        {
            _publisherManager = publisherManager;
        }

        [BindProperty]
        public Publisher Publisher { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Publisher = await _publisherManager.GetByIdAsync(id);
            if (Publisher == null)
            {
                return NotFound(); // Nếu không tìm thấy Publisher
            }

            return Page(); // Nếu tìm thấy Publisher, hiển thị trang chỉnh sửa
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Nếu form không hợp lệ, giữ lại trang Edit
            }

            await _publisherManager.UpdateAsync(Publisher); // Cập nhật Publisher
            return RedirectToPage("Index"); // Chuyển hướng về trang Index
        }
    }
}
