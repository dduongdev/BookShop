using Entities;
using Infrastructure.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UseCases;

namespace Infrastructure.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly FeedbackManager _feedbackManager;

        public FeedbackController(FeedbackManager feedbackManager)
        {
            _feedbackManager = feedbackManager;
        }

        public async Task<IActionResult> Add(FeedbackAddRequestVM feedbackAddRequestVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var feedback = new Feedback
                    {
                        BookId = feedbackAddRequestVM.BookId,
                        Rating = feedbackAddRequestVM.Rating,
                        Comment = feedbackAddRequestVM.Comment,
                        CreatedAt = DateTime.UtcNow,
                        UserId = userId
                    };

                    await _feedbackManager.AddAsync(feedback);
                    return RedirectToAction("Details", "Book", new { Id = feedbackAddRequestVM.BookId });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
