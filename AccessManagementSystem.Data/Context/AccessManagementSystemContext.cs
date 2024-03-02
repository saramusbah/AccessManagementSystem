using AccessManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccessManagementSystem.Data.Context
{
    public class AccessManagementSystemContext : IdentityDbContext<User>
    {
        public AccessManagementSystemContext(DbContextOptions<AccessManagementSystemContext> options) : base(options)
        {
        }

        public DbSet<Door> Doors { get; set; }

        public DbSet<UserDoorEvent> UserDoorEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDoorEvent>()
           .HasKey("UserId", "DoorId", "AccessTime");

            modelBuilder.Entity<UserDoorEvent>()
                .HasOne(uda => uda.User)
                .WithMany(u => u.UserEventList)
                .HasForeignKey("UserId");

            modelBuilder.Entity<UserDoorEvent>()
                .HasOne(uda => uda.Door)
                .WithMany(d => d.UserDoorEvents)
                .HasForeignKey("DoorId");


            modelBuilder.Entity<DoorRole>()
                .HasKey("DoorId", "RoleId");

            modelBuilder.Entity<DoorRole>()
                .HasOne(dr => dr.Door)
                .WithMany(d => d.DoorRoles)
                .HasForeignKey("DoorId");

            modelBuilder.Entity<DoorRole>()
                .HasOne(dr => dr.Role)
                .WithMany()
                .HasForeignKey("RoleId");

            base.OnModelCreating(modelBuilder);
        }
    }
}
