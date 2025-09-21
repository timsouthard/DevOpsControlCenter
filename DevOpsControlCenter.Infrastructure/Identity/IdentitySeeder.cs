using DevOpsControlCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DevOpsControlCenter.Infrastructure.Identity
{
    /// <summary>
    /// Provides initial seeding logic for application identity.
    /// Ensures that required roles and demo users exist in the system
    /// so that developers and testers can log in immediately after setup.
    /// 
    /// This seeder is typically executed at application startup during 
    /// environment initialization (e.g., Development/Staging).
    /// </summary>
    public static class IdentitySeeder
    {
        /// <summary>
        /// Default roles that must always exist in the system.
        /// These can be expanded later as the application introduces
        /// more fine-grained authorization.
        /// </summary>
        private static readonly string[] Roles = ["Admin", "User"];

        /// <summary>
        /// Executes the seeding process:
        /// 1. Ensures the base roles ("Admin", "User") exist.
        /// 2. Ensures an administrative account exists with known credentials.
        /// 3. Ensures a standard user account exists with known credentials.
        /// 
        /// This is safe to run multiple times—if users or roles already exist,
        /// no changes will be made.
        /// </summary>
        /// <param name="userManager">The ASP.NET Core Identity UserManager service for managing users.</param>
        /// <param name="roleManager">The ASP.NET Core Identity RoleManager service for managing roles.</param>
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // -----------------------------------------------------------------
            // Step 1: Ensure roles exist
            // -----------------------------------------------------------------
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // -----------------------------------------------------------------
            // Step 2: Ensure the default admin user exists
            // This account is intended for initial access and management.
            // Username/Email: admin@demo.com
            // Password: Admin123!
            // -----------------------------------------------------------------
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
                // Note: In production, you would not seed static credentials.
                // Instead, an administrator should be created via secure provisioning.
            }

            // -----------------------------------------------------------------
            // Step 3: Ensure a regular demo user exists
            // This account allows testing the system as a non-admin.
            // Username/Email: user@demo.com
            // Password: User123!
            // -----------------------------------------------------------------
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
