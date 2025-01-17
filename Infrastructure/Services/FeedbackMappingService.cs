using Entities;
using Infrastructure.Models.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        public async Task<IEnumerable<FeedbackVM>> MapToFeedbackVMs(IEnumerable<Feedback> feedbacks)
        {
            var users = await _userManager.GetAllAsync();
            var userLookup = users.ToDictionary(_ => _.Id);

            var feedbackVMs = new List<FeedbackVM>();
            foreach (var feedback in feedbacks)
            {
                var feedbackVM = new FeedbackVM
                {
                    Id = feedback.Id,
                    Comment = feedback.Comment ?? "",
                    Rating = feedback.Rating,
                    Username = "Unknown username",
                    CreatedAt = feedback.CreatedAt
                };

                var user = userLookup[feedback.UserId];
                if (user != null)
                {
                    feedbackVM.Username = user.Username;
                }

                feedbackVMs.Add(feedbackVM);
            }
            
            return feedbackVMs;
        }
    }
}
