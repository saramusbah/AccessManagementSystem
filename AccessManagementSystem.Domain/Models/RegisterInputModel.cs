using System.ComponentModel.DataAnnotations;

namespace AccessManagementSystem.Domain.Models
{
    public class RegisterInputModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
