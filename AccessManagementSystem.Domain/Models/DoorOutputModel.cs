namespace AccessManagementSystem.Domain.Models
{
    public class DoorOutputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static DoorOutputModel Create(int id, string name)
        {
            return new DoorOutputModel { Id = id, Name = name };
        }
    }
}