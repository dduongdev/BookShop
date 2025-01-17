using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using UseCases;

namespace Infrastructure.Areas.Admin.Books
{
    [Authorize(Roles ="Admin")]
    public class CreateModel : PageModel
    {
        private readonly BookManager _bookManager;
        private readonly BookMappingService _bookMappingService;
        private readonly CategoryManager _categoryManager;
        private readonly PublisherManager _publisherManager;
        private readonly ImageService _imageService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(BookManager bookManager, BookMappingService bookMappingService, CategoryManager categoryManager, PublisherManager publisherManager, ImageService imageService, IWebHostEnvironment webHostEnvironment)
        {
            _bookManager = bookManager;
            _bookMappingService = bookMappingService;
            _categoryManager = categoryManager;
            _publisherManager = publisherManager;
            _imageService = imageService;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Entities.Book Book { get; set; }
        [BindProperty]
        public List<IFormFile> BookImages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var Categories = await _categoryManager.GetAllActiveAsync();
            var Publishers = await _publisherManager.GetAllActiveAsync();

            ViewData["Categories"] = new SelectList(Categories, "Id", "Name");
            ViewData["Publishers"] = new SelectList(Publishers, "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Book.ImagesDirectory =  Guid.NewGuid().ToString();
            var fullyPathImagesDirectory = Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, Constants.ImagesPath, Book.ImagesDirectory));

            await _imageService.SaveImagesToDirectoryAsync(fullyPathImagesDirectory.ToString(), BookImages);

            await _bookManager.AddAsync(Book);

            return RedirectToPage("./Index");
        }
    }
}
