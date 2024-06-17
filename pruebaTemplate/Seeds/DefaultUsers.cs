using Microsoft.AspNetCore.Identity;
using PlanillaPM.Constants;
using PlanillaPM.Models;
using System.Security.Claims;

namespace PlanillaPM.Seeds
{

    public static class DefaultUsers
    {
        public static async Task SeedSuperAdminAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new Usuario
            {
                UserName = "superadmin@gmail.com",
                Email = "superadmin@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                var result = await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                }
                else
                {
                    throw new Exception("Error creating super admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(user, Roles.SuperAdmin.ToString()))
                {
                    await userManager.AddToRoleAsync(user, Roles.SuperAdmin.ToString());
                }
            }

            await roleManager.SeedClaimsForSuperAdmin();
        }

        private static async Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            var allClaims = await roleManager.GetClaimsAsync(adminRole);
            var allPermissions = Permissions.GeneratePermissionsForModule();

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == Roles.SuperAdmin.ToString() && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(adminRole, new Claim(Roles.SuperAdmin.ToString(), permission));
                }
            }
        }

        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsForModule();

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == role.Name && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim(role.Name, permission));
                }
            }
        }
    }


}
