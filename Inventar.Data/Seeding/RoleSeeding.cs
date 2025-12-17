using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Inventar.Common.Messages.ValidationConstants;

namespace Inventar.Data.Seeding
{
    public class RoleSeeding
    {
        public static async Task SeedIdentityAsync(IServiceProvider serviceProvider)
        {
            await SeedRolesAsync(serviceProvider);
            await SeedAdminAsync(serviceProvider);
        }
        private static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            string[] roles = { Admin, User };
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role}");
                    }
                }
            }
        }
        private static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var adminConfiguration = serviceProvider.GetRequiredService<IConfiguration>();

            string adminUserEmail = adminConfiguration["AdminSettings:Username"];
            string adminUserPassword = adminConfiguration["AdminSettings:Password"];

            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    Email = adminUserEmail,
                    UserName = adminUserEmail,
                    EmailConfirmed = false
                };

                var result = await userManager.CreateAsync(adminUser, adminUserPassword);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user {adminUserEmail}");
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, Admin))
            {
                var result = await userManager.AddToRoleAsync(adminUser, Admin);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to add {adminUserEmail} to Admin role");
                }
            }
        }
    }
}
