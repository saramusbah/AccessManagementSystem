namespace AccessManagementSystem.Domain.Models
{
    public class AccessHistoryOutputModel
    {
        public string userId { get; set; }

        public int DoorId { get; set; }

        public DateTime AccessTime { get; set; }

        public bool IsSuccess { get; set; }

        public string AccessMethod { get; set; }
    }
}