using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models.ViewModels
{
    public class FeedbackAddRequestVM
    {
        public string? Comment { get; set; }
        [Required(ErrorMessage = "Trường này là bắt buộc.")]
        public required int Rating { get; set; } = 5;
        public int BookId { get; set; }
    }
}
