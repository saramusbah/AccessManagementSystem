using AccessManagementSystem.Domain.Models;

namespace AccessManagementSystem.Domain.Contracts
{
    public interface IDoorService
    {
        Task<IEnumerable<DoorOutputModel>> GetDoors();

        Task<bool> DoorExists(int id);

        Task CreateDoor(CreateDoorInputModel model);

        Task SetDoorRole(int doorId, string roleName);

        Task<bool> DoorRoleExists(int doorId, string roleName);
    }
}
