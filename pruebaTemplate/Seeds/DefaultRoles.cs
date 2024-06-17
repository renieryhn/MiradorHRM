using Microsoft.AspNetCore.Identity;
using PlanillaPM.Constants;
using PlanillaPM.Models;

namespace PlanillaPM.Seeds
{
    public static class DefaultRoles
    {

        public static async Task SeedAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Verifica si el rol SuperAdmin existe
            if (!await roleManager.RoleExistsAsync(Roles.SuperAdmin.ToString()))
            {
                // Crea el rol SuperAdmin si no existe
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            }
        }

    }
}
