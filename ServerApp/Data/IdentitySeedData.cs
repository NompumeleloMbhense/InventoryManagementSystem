using Microsoft.AspNetCore.Identity;
using ServerApp.Models;

/// <summary>
/// Seeds initial roles and users into the identity database.
/// Creates Admin and User roles, and default admin and user accounts.
/// </summary>

namespace ServerApp.Data
{
    public static class IdentitySeedData
    {
        public static async Task SeedRolesAndUsersAsync(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config)
        {
            // 1. Seed Roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Seed Admin
            await CreateUserIfNotExists(userManager, config, "AdminEmail", "AdminPassword", "Administrator", "Admin");

            // 3. Seed Regular User
            await CreateUserIfNotExists(userManager, config, "UserEmail", "UserPassword", "Test User", "User");
        }

        private static async Task CreateUserIfNotExists(
            UserManager<AppUser> userManager,
            IConfiguration config,
            string emailKey,
            string passwordKey,
            string fullName,
            string role)
        {
            var email = config[$"SeedUser:{emailKey}"];
            var password = config[$"SeedUser:{passwordKey}"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return; // Skip if config is missing rather than crashing the whole app

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true // Useful for bypassing verification in dev
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
