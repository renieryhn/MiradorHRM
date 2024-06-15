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
            //Seed Default User
            var defaultUser = new Usuario
            {
                UserName = "superadmin@gmail.com",
                Email = "superadmin@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            // Verifica si el usuario ya existe por correo electrónico
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                // Crea el usuario si no existe
                var result = await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                if (result.Succeeded)
                {
                    // Añade el usuario al rol SuperAdmin
                    await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                }
                else
                {
                    // Manejo de errores de creación de usuario
                    throw new Exception("Error creating super admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                // Añade el usuario al rol SuperAdmin si ya existe pero no está en el rol
                if (!await userManager.IsInRoleAsync(user, Roles.SuperAdmin.ToString()))
                {
                    await userManager.AddToRoleAsync(user, Roles.SuperAdmin.ToString());
                }
            }

            // Seed claims for SuperAdmin role
            await roleManager.SeedClaimsForSuperAdmin();
        }

        
        private static async Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());

            var allClaims = await roleManager.GetClaimsAsync(adminRole);
            var module = "Rol";
            var allPermissions = Permissions.GeneratePermissionsForModule(module);

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(adminRole, new Claim("Permission", permission));
                }
            }
        }

        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }
    }
}
