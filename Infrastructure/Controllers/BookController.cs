using Entities;
using Infrastructure.Models;
using Infrastructure.Models.ViewModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using UseCases;

namespace Infrastructure.Controllers
{
    public class BookController : Controller
    {
        private readonly BookManager _bookManager;
        private readonly PublisherManager _publisherManager;
        private readonly CategoryManager _categoryManager;
        private readonly FeedbackManager _feedbackManager;
        private readonly OrderManager _orderManager;
        private readonly BookProcessingService _bookProcessingService;
        private readonly BookMappingService _bookMappingService;
        private readonly FeedbackMappingService _feedbackMappingService;

        public BookController(BookManager bookManager, PublisherManager publisherManager, CategoryManager categoryManager, OrderManager orderManager, FeedbackManager feedbackManager, BookProcessingService bookProcessingService, BookMappingService bookMappingService, FeedbackMappingService feedbackMappingService)
        {
            _bookManager = bookManager;
            _publisherManager = publisherManager;
            _orderManager = orderManager;
            _categoryManager = categoryManager;
            _feedbackManager = feedbackManager;
            _bookProcessingService = bookProcessingService;
            _bookMappingService = bookMappingService;
            _feedbackMappingService = feedbackMappingService;
        }

        public async Task<IActionResult> Index(BookFilterCriteria filterCriteria, string? titleSearchKeyword, BookSortCriteria sortCriteria, int pageIndex = 1)
        {
            if (pageIndex <= 0)
            {
                return BadRequest();
            }

            var activeBooks = await _bookManager.GetAllActivateAsync();

            activeBooks = _bookProcessingService.ApplyFiltering(activeBooks, filterCriteria);

            activeBooks = activeBooks.Where(_ => string.IsNullOrEmpty(titleSearchKeyword) ? true : _.Title.Contains(titleSearchKeyword, StringComparison.OrdinalIgnoreCase));

            activeBooks = _bookProcessingService.ApplySorting(activeBooks, sortCriteria);

            PaginationService<Book> paginationService = new PaginationService<Book>(activeBooks, 12);
            IEnumerable<Book> paginatedActivateBooks = paginationService.GetItemsByPage(pageIndex);

            ViewBag.PaginationMetadata = paginationService.GetPaginationMetadata();

            var categories = await _categoryManager.GetAllActiveAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            var publishers = await _publisherManager.GetAllActiveAsync();
            ViewBag.Publishers = new SelectList(publishers, "Id", "Name");

            IEnumerable<BookCardVM> bookCardVMs = await _bookMappingService.MapToBookCardVMs(paginatedActivateBooks);

            return View(bookCardVMs);
        }

        public async Task<IActionResult> DetailsAsync(int? id)
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

            var bookDetailsVM = await _bookMappingService.MapToBookDetailsVM(foundBook);

            var feedbacks = await _feedbackManager.GetByBookIdAsync(id.Value);
            var feedbackVMs = await _feedbackMappingService.MapToFeedbackVMs(feedbacks);
            ViewBag.Feedbacks = feedbackVMs;

            var averageRating = 5.0;
            if (feedbacks.Any())
            {
                averageRating = (5 + feedbacks.Average(_ => _.Rating)) / 2;
            }
            
            ViewBag.AverageRating = averageRating;
            ViewBag.TotalRating = feedbacks.Count();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hasPurchased = userId != null && (await _orderManager.GetAllAsync())
                .Any(order => order.UserId == int.Parse(userId) && order.OrderItems.Any(item => item.BookId == id.Value));
            ViewBag.HasPurchased = hasPurchased;


            return View(bookDetailsVM);
        }
    }
}
