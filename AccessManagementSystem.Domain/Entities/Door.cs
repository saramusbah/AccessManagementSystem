namespace AccessManagementSystem.Domain.Entities
{
    public class Door
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserDoorEvent> UserAccessList { get; set; } = new HashSet<UserDoorEvent>();

        public ICollection<DoorRole> DoorRoles { get; set; } = new HashSet<DoorRole>();
    }
}
