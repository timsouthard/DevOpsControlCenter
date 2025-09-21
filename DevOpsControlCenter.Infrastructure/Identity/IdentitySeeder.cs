using DevOpsControlCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DevOpsControlCenter.Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        private static readonly string[] Roles = ["Admin", "User"];

        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure roles exist
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin user
            var adminEmail = "admin@demo.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    DisplayName = "Admin"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Regular user
            var userEmail = "user@demo.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var normalUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    DisplayName = "Demo User"
                };

                var result = await userManager.CreateAsync(normalUser, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "User");
                }
            }
        }
    }
}
