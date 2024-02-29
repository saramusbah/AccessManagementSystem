namespace AccessManagementSystem.Domain.Entities
{
    public class UserDoorEvent
    {
        public User User { get; set; }

        public Door Door { get; set; }

        public bool IsSuccess { get; set; }

        public DateTime AccessTime { get; set; }

        public AccessMethod AccessMethod { get; set; }

        public static UserDoorEvent Create(string userId, int doorId, bool isSuccess, DateTime accessTime, AccessMethod accessMethod)
        {
            return new UserDoorEvent
            {
                User = new User { Id = userId },
                Door = new Door { Id = doorId },
                IsSuccess = isSuccess,
                AccessTime = accessTime,
                AccessMethod = accessMethod,
            };
        }
    }
}
