using AccessManagementSystem.Data.Context;
using AccessManagementSystem.Domain.Contracts;
using AccessManagementSystem.Domain.Entities;
using AccessManagementSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessManagementSystem.Data.Services
{
    public class DoorService : IDoorService
    {
        private readonly AccessManagementSystemContext _dbContext;

        public DoorService(AccessManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateDoor(CreateDoorInputModel model)
        {
            var newDoor = new Door { Name = model.Name };
            await _dbContext.Doors.AddAsync(newDoor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DoorExists(int id)
        {
            return await _dbContext.Doors.AsNoTracking().SingleOrDefaultAsync(d => d.Id == id) != null;
        }

        public async Task<IEnumerable<DoorOutputModel>> GetDoors()
        {
            return await _dbContext.Doors.AsNoTracking().Select(d => DoorOutputModel.Create(d.Id, d.Name)).ToListAsync();
        }

        public async Task SetDoorRole(int doorId, string roleName)
        {
            var door = await _dbContext.Doors.SingleAsync(d => d.Id == doorId);
            var role = await _dbContext.Roles.SingleAsync(r => r.Name == roleName);
            door.DoorRoles.Add(new DoorRole { Door = door, Role = role });
            await _dbContext.SaveChangesAsync();
        }
    }
}