namespace AccessManagementSystem.Domain.Entities
{
    public class UserDoorEvent
    {
        public User User { get; set; }

        public Door Door { get; set; }

        public bool IsSuccess { get; set; }

        public DateTime AccessTime { get; set; }

        public AccessMethod AccessMethod { get; set; }

        public static UserDoorEvent Create(User user, Door door, bool isSuccess, AccessMethod accessMethod)
        {
            return new UserDoorEvent
            {
                User = user,
                Door = door,
                IsSuccess = isSuccess,
                AccessTime = DateTime.UtcNow,
                AccessMethod = accessMethod,
            };
        }
    }
}
