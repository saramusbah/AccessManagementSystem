using System.ComponentModel.DataAnnotations;

namespace AccessManagementSystem.Domain.Models
{
    public class SetDoorToRoleInputModel
    {
        [Required]
        public string RoleName {  get; set; }
    }
}
