using AccessManagementSystem.Data.Context;
using AccessManagementSystem.Domain.Contracts;
using AccessManagementSystem.Domain.Entities;
using AccessManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessManagementSystem.Data.Services
{
    public class AccessService : IAccessService
    {
        private readonly AccessManagementSystemContext _dbContext;

        public AccessService(AccessManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CanGrantAccessAsync(string userId, int doorId)
        {
            var door = await _dbContext.Doors.AsNoTracking().Include(d => d.DoorRoles).ThenInclude(dr => dr.Role).AsNoTracking().SingleAsync(d => d.Id == doorId);
            var roles = await _dbContext.UserRoles.AsNoTracking().Where(r => r.UserId == userId).ToListAsync();

            return door.DoorRoles.Any(dr => roles.Any(ur => ur.RoleId == dr.Role.Id));
        }

        public async Task<IEnumerable<AccessHistoryOutputModel>> GetDoorAccessHistoryAsync(int doorId)
        {
            return await _dbContext.UserDoorEvents.AsNoTracking().Where(ude => ude.Door.Id == doorId).Select(de =>
                      new AccessHistoryOutputModel
                      {
                          DoorId = de.Door.Id,
                          userId = de.User.Id,
                          AccessMethod = de.AccessMethod.ToString(),
                          AccessTime = de.AccessTime,
                          IsSuccess = de.IsSuccess
                      }).ToListAsync();
        }

        public async Task<IEnumerable<AccessHistoryOutputModel>> GetUserAccessHistoryAsync(string userId)
        {
            return await _dbContext.UserDoorEvents.AsNoTracking().Where(ude => ude.User.Id == userId).Select(de =>
                      new AccessHistoryOutputModel
                      {
                          DoorId = de.Door.Id,
                          userId = de.User.Id,
                          AccessMethod = de.AccessMethod.ToString(),
                          AccessTime = de.AccessTime,
                          IsSuccess = de.IsSuccess
                      }).ToListAsync();
        }

        public async Task LogRemoteUserDoorEventAsync(string userId, int doorId, bool isSuccessful)
        {
            var door = await _dbContext.Doors.SingleAsync(d => d.Id == doorId);
            var user = await _dbContext.Users.SingleAsync(r => r.Id == userId);
            door.UserDoorEvents.Add(UserDoorEvent.Create(user, door, isSuccessful, AccessMethod.Tag));
            await _dbContext.SaveChangesAsync();
        }

        public async Task LogTagUserDoorEventAsync(string userId, int doorId, bool isSuccessful)
        {
            var door = await _dbContext.Doors.SingleAsync(d => d.Id == doorId);
            var user = await _dbContext.Users.SingleAsync(r => r.Id == userId);
            door.UserDoorEvents.Add(UserDoorEvent.Create(user, door, isSuccessful, AccessMethod.Tag));
            await _dbContext.SaveChangesAsync();
        }
    }
}