using AccessManagementSystem.Data.Context;
using AccessManagementSystem.Data.Services;
using AccessManagementSystem.Domain.Contracts;
using AccessManagementSystem.Domain.Entities;
using AccessManagementSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AccessManagementSystem.Tests
{
    [TestFixture]
    public class AccessServiceIntegrationTests
    {
        private AccessManagementSystemContext _dbContext;
        private IAccessService _accessService;
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

            InitializeTestData();
            _accessService = new AccessService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task CanGrantAccessAsync_ShouldReturnTrueForEmployee()
        {
            // Arrange
            var userId = _dbContext.Users.First().Id;
            var doorId = _dbContext.Doors.First().Id;
            var roleId = _dbContext.Roles.First(r => r.Name == "Employee").Id;

            var userRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>{RoleId = roleId, UserId = userId}
            };

            _dbContext.UserRoles.AddRange(userRoles);
            _dbContext.SaveChanges();

            // Act
            var result = await _accessService.CanGrantAccessAsync(userId, doorId);

            // Assert
            Assert.IsTrue(result);
        }


        [Test]
        public async Task CanGrantAccessAsync_ShouldReturnFalseForEmployee()
        {
            // Arrange
            var userId = _dbContext.Users.First().Id;
            var doorId = _dbContext.Doors.First().Id;
            var roleId = _dbContext.Roles.First(r => r.Name == "Employee").Id;

            // Act
            var result = await _accessService.CanGrantAccessAsync(userId, doorId);

            // Assert
            Assert.IsFalse(result);
        }
        [Test]
        public async Task GetDoorAccessHistoryAsync_ShouldReturnAccessHistoryForDoor()
        {
            // Arrange
            var doorId = _dbContext.Doors.First().Id;

            // Act
            var result = await _accessService.GetDoorAccessHistoryAsync(doorId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<AccessHistoryOutputModel>>(result);
            Assert.AreEqual(1, result.Count());
        }

        private void InitializeTestData()
        {
            var doors = new List<Door>
            {
                new Door { Name = "Door1" },
                new Door { Name = "Door2" },
            };

            var users = new List<User>
            {
                new User { UserName = "TestUser1", Email = "TestUserEmail1", TokenVersion = "Test" },
                new User { UserName = "TestUser2", Email = "TestUserEmail2", TokenVersion = "Test" },
            };

            var roles = new List<IdentityRole>
            {
                new IdentityRole {Name = "Admin"},
                new IdentityRole {Name = "Employee"},
                new IdentityRole {Name = "Director"},
            };

            var doorRoles = new List<DoorRole>
            {
                new DoorRole{Door = doors[0], Role = roles[1]},
                new DoorRole{Door = doors[1], Role = roles[2]},
            };

            doors[0].DoorRoles.Add(doorRoles[0]);
            doors[1].DoorRoles.Add(doorRoles[1]);

            var userDoorEvents = new List<UserDoorEvent>
            {
                new UserDoorEvent
                {
                    Door = doors[0],
                    User = users[0],
                    AccessMethod = AccessMethod.Tag,
                    AccessTime = DateTime.UtcNow,
                    IsSuccess = true
                },
            };

            _dbContext.Doors.AddRange(doors);
            _dbContext.Users.AddRange(users);
            _dbContext.Roles.AddRange(roles);
            _dbContext.UserDoorEvents.AddRange(userDoorEvents);

            _dbContext.SaveChanges();
        }
    }
}