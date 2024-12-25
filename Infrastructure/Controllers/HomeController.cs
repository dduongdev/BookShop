using Entities;
using Infrastructure.Models;
using Infrastructure.Models.ViewModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using UseCases;
using UseCases.Repositories;

namespace Infrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookManager _bookManager;
        private readonly BookMappingService _bookMappingService;
        private readonly ImageService _imageService;

        public HomeController(
           BookManager bookManager,
           BookMappingService bookMappingService,
           ImageService imageService)
        {
            _bookManager = bookManager;
            _bookMappingService = bookMappingService;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            if (pageIndex <= 0)
            {
                return BadRequest();
            }

            var activeBooks = await _bookManager.GetAllActivateAsync();
            RequestScopedPaginationService<Book> paginationService = new RequestScopedPaginationService<Book>(activeBooks, 12);
            IEnumerable<Book> paginatedActiveBooks = paginationService.GetItemsByPage(pageIndex);

            IEnumerable<BookCardVM> bookCardVMs = await _bookMappingService.MapToBookCardVMs(activeBooks);

            IEnumerable<string> banners = _imageService.GetAllFromImagesDirectory(Constants.BannersDirectory);
            if (!banners.Any())
            {
                banners = new List<string> { Constants.DefaultImagePath };
            }
            ViewBag.Banners = banners;

            ViewBag.PaginationMetadata = paginationService.GetPaginationMetadata();

            return View(bookCardVMs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
