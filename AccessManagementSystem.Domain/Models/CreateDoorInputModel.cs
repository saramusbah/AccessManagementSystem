using System.ComponentModel.DataAnnotations;

namespace AccessManagementSystem.Domain.Models
{
    public class CreateDoorInputModel
    {
        [Required]
        public string Name { get; set; }
    }
}