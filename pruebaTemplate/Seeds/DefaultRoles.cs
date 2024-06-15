using Microsoft.AspNetCore.Identity;
using PlanillaPM.Constants;
using PlanillaPM.Models;

namespace PlanillaPM.Seeds
{
    public static class DefaultRoles
    {

        public static async Task SeedAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Roles.SuperAdmin.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            }
        }
        //public static async Task SeedAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        //{
        //    await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
        //    //await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
        //    //await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
        //}
    }
}
