using AccessManagementSystem.Data.Context;
using AccessManagementSystem.Data.Services;
using AccessManagementSystem.Domain.Entities;
using AccessManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AccessManagementSystem.Tests
{
    [TestFixture]
    public class DoorServiceTests
    {
        private AccessManagementSystemContext _dbContext;
        private IConfigurationRoot _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.test.json")
            .Build();

            var connectionString = _configuration.GetConnectionString("TestDB");
            var options = new DbContextOptionsBuilder<AccessManagementSystemContext>()
                .UseSqlServer(connectionString)
                .Options;

            _dbContext = new AccessManagementSystemContext(options);
            _dbContext.Database.EnsureCreated();
        }

        [TearDown]
        public void Teardown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task CreateDoor_ShouldAddDoorToDatabase()
        {
            // Arrange
            var doorService = new DoorService(_dbContext);
            var model = new CreateDoorInputModel { Name = "TestDoor" };

            // Act
            await doorService.CreateDoor(model);

            // Assert
            var door = await _dbContext.Doors.SingleOrDefaultAsync(d => d.Name == "TestDoor");
            Assert.NotNull(door);
        }

        [Test]
        public async Task DoorExists_ShouldReturnTrueForExistingDoor()
        {
            // Arrange
            var doorService = new DoorService(_dbContext);
            var model = new CreateDoorInputModel { Name = "ExistingDoor" };
            await doorService.CreateDoor(model);

            // Act
            var doorExists = await doorService.DoorExists(_dbContext.Doors.First().Id);

            // Assert
            Assert.True(doorExists);
        }

        [Test]
        public async Task DoorExists_ShouldReturnFalseForNonExistingDoor()
        {
            // Arrange
            var doorService = new DoorService(_dbContext);

            // Act
            var doorExists = await doorService.DoorExists(123);

            // Assert
            Assert.False(doorExists);
        }

        [Test]
        public async Task GetDoors_ShouldReturnListOfDoorOutputModel()
        {
            // Arrange
            var doorService = new DoorService(_dbContext);
            await doorService.CreateDoor(new CreateDoorInputModel { Name = "Door1" });
            await doorService.CreateDoor(new CreateDoorInputModel { Name = "Door2" });

            // Act
            var doors = await doorService.GetDoors();

            // Assert
            Assert.IsInstanceOf<IEnumerable<DoorOutputModel>>(doors);
            Assert.AreEqual(2, doors.Count());
        }

        [Test]
        public async Task SetDoorRole_ShouldAddRoleToDoor()
        {
            // Arrange
            var doorService = new DoorService(_dbContext);
            var role = new IdentityRole { Name = "Admin" };
            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();

            var door = new Door { Name = "TestDoor" };
            _dbContext.Doors.Add(door);
            await _dbContext.SaveChangesAsync();

            // Act
            await doorService.SetDoorRole(door.Id, "Admin");

            // Assert
            var doorWithRole = await _dbContext.Doors
                .Include(d => d.DoorRoles)
                .ThenInclude(dr => dr.Role)
                .SingleOrDefaultAsync(d => d.Id == door.Id);

            Assert.NotNull(doorWithRole);
            Assert.AreEqual(1, doorWithRole.DoorRoles.Count);
            Assert.AreEqual("Admin", doorWithRole.DoorRoles.First().Role.Name);
        }
    }
}