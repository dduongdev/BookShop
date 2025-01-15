using Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using UseCases;

namespace Areas.Admin.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly BookManager _bookManager;

        // Biến để chứa danh sách sách
        public List<Book> Books { get; set; }

        // Constructor để inject BookManager vào
        public IndexModel(BookManager bookManager)
        {
            _bookManager = bookManager;
        }

        // Phương thức xử lý GET request
        public async Task OnGetAsync()
        {
            // Lấy danh sách sách từ BookManager
            Books = (await _bookManager.GetAllAsync()).ToList();
        }
    }
}
