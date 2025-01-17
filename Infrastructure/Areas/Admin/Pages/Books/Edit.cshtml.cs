using Entities.Enums;
using Infrastructure.Models;
using Infrastructure.Services;
using Infrastructure.SqlServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using UseCases;

namespace Infrastructure.Areas.Admin.Books
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly BookManager _bookManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageService _imageService;
        private readonly CategoryManager _categoryManager;
        private readonly PublisherManager _publisherManager;

        public EditModel(BookManager bookManager, IWebHostEnvironment webHostEnvironment, ImageService imageService, CategoryManager categoryManager, PublisherManager publisherManager)
        {
            _bookManager = bookManager;
            _webHostEnvironment = webHostEnvironment;
            _imageService = imageService;
            _categoryManager = categoryManager;
            _publisherManager = publisherManager;
        }

        [BindProperty]
        public Entities.Book Book { get; set; } = default!;

        [BindProperty]
        public List<string> StoredImages { get; set; } = default!;
        [BindProperty]
        public List<IFormFile> UploadedImages { get; set; } = default!;
        [BindProperty]
        public List<string> DeletedImages { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var foundBook = await _bookManager.GetByIdAsync(id.Value);

            if (foundBook == null)
            {
                return NotFound();
            }

            if (foundBook.ImagesDirectory != null)
            {
                StoredImages = _imageService.GetAllFromImagesDirectory(foundBook.ImagesDirectory).ToList();
            }

            var Categories = await _categoryManager.GetAllActiveAsync();
            var Publishers = await _publisherManager.GetAllActiveAsync();

            ViewData["Categories"] = new SelectList(Categories, "Id", "Name");
            ViewData["Publishers"] = new SelectList(Publishers, "Id", "Name");

            Book = foundBook;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Book == null)
            {
                return BadRequest();
            }

            var foundBook = await _bookManager.GetByIdAsync(Book.Id);
            if (foundBook == null)
            {
                return NotFound();
            }

            foundBook.Title = Book.Title;
            foundBook.Author = Book.Author;
            foundBook.Description = Book.Description;
            foundBook.Price = Book.Price;
            foundBook.DiscountPercentage = Book.DiscountPercentage;
            foundBook.CategoryId = Book.CategoryId;
            foundBook.PublisherId = Book.PublisherId;
            foundBook.Stock = Book.Stock;

            DeletedImages = DeletedImages.Select(image => image.Replace("/", "\\")).ToList();
            DeletedImages = DeletedImages.Select(image => image.TrimStart(Path.DirectorySeparatorChar, Path.DirectorySeparatorChar)).ToList();
            _imageService.DeleteImagesFromDirectoryAsync(_webHostEnvironment.WebRootPath, DeletedImages);

            if (foundBook.ImagesDirectory == null)
            {
                foundBook.ImagesDirectory = Guid.NewGuid().ToString();
                if (!Directory.Exists(foundBook.ImagesDirectory))
                {
                    Directory.CreateDirectory(foundBook.ImagesDirectory);
                }
            }

            var fullyImagesDirectoryPath = Path.Combine(_webHostEnvironment.WebRootPath, Constants.ImagesPath, foundBook.ImagesDirectory);
            await _imageService.SaveImagesToDirectoryAsync(fullyImagesDirectoryPath, UploadedImages);

            await _bookManager.UpdateAsync(foundBook);

            return RedirectToPage("/Books/Index");
        }
    }
}
