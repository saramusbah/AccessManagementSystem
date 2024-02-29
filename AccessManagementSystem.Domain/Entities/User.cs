
using Microsoft.AspNetCore.Identity;

namespace AccessManagementSystem.Domain.Entities
{
    public class User : IdentityUser
    {
        public string TokenVersion { get; set; }

        public ICollection<UserDoorEvent> UserEventList { get; set; } = new HashSet<UserDoorEvent>();
    }
}
