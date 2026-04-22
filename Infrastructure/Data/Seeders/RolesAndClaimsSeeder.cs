using DBS_Task.Application.Common.Constants;
using DBS_Task.Application.Mappings;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DBS_Task.Infrastructure.Data.Seeders
{
    public static class RolesAndClaimsSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in AppRoles.AllRoles)
            {
                // 1. Create Role if not exists
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var newRole = new IdentityRole(roleName);
                    await roleManager.CreateAsync(newRole);
                }

                var role = await roleManager.FindByNameAsync(roleName);

                // 2. Get existing claims
                var existingClaims = await roleManager.GetClaimsAsync(role);

                // 3. Add missing claims
                var claims = RoleClaimsMapping.Mapping[roleName];

                // Only add claims that are not already associated with the role
                foreach (var claim in claims)
                {
                    if (!existingClaims.Any(c => c.Type == "permission" && c.Value == claim))
                    {
                        await roleManager.AddClaimAsync(role, new Claim("permission", claim));
                    }
                }
            }
        }
    }
}
