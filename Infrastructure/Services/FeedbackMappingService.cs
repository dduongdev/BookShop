using Entities;
using Infrastructure.Models.ViewModels;
using UseCases;

namespace Infrastructure.Services
{
    public class FeedbackMappingService
    {
        private readonly UserManager _userManager;

        public FeedbackMappingService(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<FeedbackVM> MapToFeedbackVM(Feedback feedback)
        {
            var feedbackVM = new FeedbackVM
            {
                Id = feedback.Id,
                Comment = feedback.Comment ?? "",
                Rating = feedback.Rating,
                Username = "Unknown username",
                CreatedAt = feedback.CreatedAt
            };

            var user = await _userManager.GetByIdAsync(feedback.UserId);
            if (user != null)
            {
                feedbackVM.Username = user.Username;
            }

            return feedbackVM;
        }
    }
}
