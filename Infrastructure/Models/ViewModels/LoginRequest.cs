using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models.ViewModels
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username là bắt buộc.")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "Password là bắt buộc.")]
        public string Password { get; set; } = default!;
    }
}
