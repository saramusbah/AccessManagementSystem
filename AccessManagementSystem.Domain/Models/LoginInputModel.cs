using System.ComponentModel.DataAnnotations;

namespace AccessManagementSystem.Domain.Models
{
    public class LoginInputModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
