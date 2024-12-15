﻿using Entities;
using Infrastructure.Models;
using Infrastructure.Models.ViewModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UseCases;

namespace Infrastructure.Controllers
{
    public class BookController : Controller
    {
        private readonly BookManager _bookManager;
        private readonly PublisherManager _publisherManager;
        private readonly CategoryManager _categoryManager;
        private readonly FeedbackManager _feedbackManager;
        private readonly BookProcessingService _bookProcessingService;
        private readonly BookMappingService _bookMappingService;

        public BookController(BookManager bookManager, PublisherManager publisherManager, CategoryManager categoryManager, FeedbackManager feedbackManager, BookProcessingService bookProcessingService, BookMappingService bookMappingService)
        {
            _bookManager = bookManager;
            _publisherManager = publisherManager;
            _categoryManager = categoryManager;
            _feedbackManager = feedbackManager;
            _bookProcessingService = bookProcessingService;
            _bookMappingService = bookMappingService;
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

            RequestScopedPaginationService<Book> paginationService = new RequestScopedPaginationService<Book>(activeBooks, 12);
            IEnumerable<Book> paginatedActivateBooks = paginationService.GetItemsByPage(pageIndex);

            ViewBag.PaginationMetadata = paginationService.GetPaginationMetadata();

            var categories = await _categoryManager.GetAllActiveAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            var publishers = await _publisherManager.GetAllActiveAsync();
            ViewBag.Publishers = new SelectList(publishers, "Id", "Name");

            IEnumerable<BookCardVM> bookCardVMs = await Task.WhenAll(paginatedActivateBooks.Select(book => _bookMappingService.MapToBookCardVM(book)));

            return View(bookCardVMs);
        }
    }
}