using Microsoft.AspNetCore.Identity;

namespace AccessManagementSystem.Domain.Entities
{
    public class DoorRole
    {
        public Door Door { get; set; }

        public IdentityRole Role { get; set; }
    }
}
