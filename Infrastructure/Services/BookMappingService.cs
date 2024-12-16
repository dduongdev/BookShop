using Entities;
using Infrastructure.Models;
using Infrastructure.Models.ViewModels;
using System.Reflection.Metadata;
using UseCases;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Services
{
    public class BookMappingService
    {
        private readonly FeedbackManager _feedbackManager;
        private readonly PublisherManager _publisherManager;
        private readonly CategoryManager _categoryManager;
        private readonly ImageService _imageService;

        public BookMappingService(FeedbackManager feedbackManager, CategoryManager categoryManager, PublisherManager publisherManager, ImageService imageService)
        {
            _feedbackManager = feedbackManager;
            _publisherManager = publisherManager;
            _categoryManager = categoryManager;
            _imageService = imageService;
        }

        public async Task<BookCardVM> MapToBookCardVM(Book book)
        {
            var bookCardVM = new BookCardVM 
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author ?? "Unknown author",
                Price = book.Price,
                DiscountPercentage = book.DiscountPercentage,
                Stock = book.Stock,
                Image = Constants.DefaultImagePath,
                PublisherName = "Unknown publisher",
                Rating = 5
            };

            var publisher = await _publisherManager.GetByIdAsync(book.PublisherId);
            if (publisher != null)
            {
                bookCardVM.PublisherName = publisher.Name;
            }

            if (!string.IsNullOrEmpty(book.ImagesDirectory))
            {
                var images = _imageService.GetAllFromImagesDirectory(book.ImagesDirectory);
                bookCardVM.Image = images.FirstOrDefault() ?? Constants.DefaultImagePath;
            }

            var feedbacks = await _feedbackManager.GetByBookIdAsync(book.Id);
            if (feedbacks != null && feedbacks.Any())
            {
                bookCardVM.Rating = (bookCardVM.Rating + feedbacks.Average(_ => _.Rating)) / 2;
            }

            return bookCardVM;
        }

        public async Task<BookDetailsVM> MapToBookDetailsVM(Book book)
        {
            var bookDetailsVM = new BookDetailsVM 
            { 
                Id = book.Id,
                Title = book.Title,
                Author = book.Author ?? "Unknown author",
                Description = book.Description ?? "No description",
                CategoryName = "Unknown category",
                PublisherName = "Unknown publisher",
                DiscountPercentage = book.DiscountPercentage,
                Price = book.Price,
                Stock = book.Stock,
                Status = book.Status
            };

            ((List<string>)bookDetailsVM.Images).Add(Constants.DefaultImagePath);
            if (book.ImagesDirectory != null)
            {
                var images = _imageService.GetAllFromImagesDirectory(book.ImagesDirectory);
                if (images != null && images.Any())
                {
                    bookDetailsVM.Images = images;
                }
            }

            var publisher = await _publisherManager.GetByIdAsync(book.PublisherId);
            if (publisher != null)
            {
                bookDetailsVM.PublisherName = publisher.Name;
            }

            var category = await _categoryManager.GetByIdAsync(book.CategoryId);
            if (category != null)
            {
                bookDetailsVM.CategoryName = category.Name;
            }

            return bookDetailsVM;
        }
    }
}
