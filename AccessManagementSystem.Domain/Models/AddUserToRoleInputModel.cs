using System.ComponentModel.DataAnnotations;

namespace AccessManagementSystem.Domain.Models
{
    public class AddUserToRoleInputModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}