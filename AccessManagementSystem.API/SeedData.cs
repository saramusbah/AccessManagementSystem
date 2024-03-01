using AccessManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AccessManagementSystem.API
{
    public static class SeedData
    {
        public static async Task Initialize(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            string[] roleNames = { "Admin", "Employee", "Director" };

            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = configuration["AdminUser:Email"];
            string adminPassword = configuration["AdminUser:Password"];

            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                TokenVersion = Guid.NewGuid().ToString()
            };

            var user = await userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);

                if (createAdminUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}