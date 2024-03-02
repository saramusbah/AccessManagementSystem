using AccessManagementSystem.Domain.Models;

namespace AccessManagementSystem.Domain.Contracts
{
    public interface IAccessService
    {
        Task<IEnumerable<AccessHistoryOutputModel>> GetUserAccessHistoryAsync(string userId);

        Task<IEnumerable<AccessHistoryOutputModel>> GetDoorAccessHistoryAsync(int doorId);

        Task LogRemoteUserDoorEventAsync(string userId, int doorId, bool isSuccessful);

        Task LogTagUserDoorEventAsync(string userId, int doorId, bool isSuccessful);

        Task<bool> CanGrantAccessAsync(string userId, int doorId);
    }
}