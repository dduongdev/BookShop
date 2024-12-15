using Entities;
using Infrastructure.Models;
using Infrastructure.Models.ViewModels;
using System.Reflection.Metadata;
using UseCases;

namespace Infrastructure.Services
{
    public class BookMappingService
    {
        private readonly FeedbackManager _feedbackManager;
        private readonly PublisherManager _publisherManager;
        private readonly ImageService _imageService;

        public BookMappingService(FeedbackManager feedbackManager, PublisherManager publisherManager, ImageService imageService)
        {
            _feedbackManager = feedbackManager;
            _publisherManager = publisherManager;
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
    }
}
