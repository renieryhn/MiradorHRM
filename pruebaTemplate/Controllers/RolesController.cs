using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Grid;
using System.Security.Claims;
using PlanillaPM.Constants;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace PlanillaPM.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Usuario> userManager;
        private readonly PlanillaContext context;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<Usuario> userManager, PlanillaContext context)
        {
            _roleManager = roleManager;
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
            {
                // Crear el rol
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));

                if (result.Succeeded)
                {
                    // Generar permisos para el nuevo rol
                    var permissions = Permissions.GeneratePermissionsForModule(roleName.Trim());

                    // Obtener el rol recién creado
                    var newRole = await _roleManager.FindByNameAsync(roleName.Trim());

                    // Agregar reclamaciones para cada permiso generado
                    foreach (var permission in permissions)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(CustomClaimTypes.Permission, permission));
                    }

                    // Obtener el rol de superadministrador
                    var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");

                    // Asignar los mismos permisos al rol de superadministrador
                    if (superAdminRole != null)
                    {
                        foreach (var permission in permissions)
                        {
                            await _roleManager.AddClaimAsync(superAdminRole, new Claim(CustomClaimTypes.Permission, permission));
                        }
                    }

                    // Configurar el mensaje de éxito
                    TempData["success"] = $"Nuevo Rol: {roleName.Trim()}";
                }
                else
                {
                    // Manejar error de creación de rol
                    TempData["error"] = $"Error al crear el rol: {result.Errors.FirstOrDefault()?.Description ?? "Unknown error"}";
                }
            }
            else
            {
                // Manejar nombre de rol vacío
                TempData["error"] = "Nombre de rol no válido.";
            }

            // Redireccionar a la vista Index
            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> AddRole(string roleName)
        //{
        //    if (roleName != null)
        //    {
        //        // Crear el rol
        //        var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));

        //        if (result.Succeeded)
        //        {
        //            // Generar permisos para el nuevo rol
        //            var permissions = Permissions.GeneratePermissionsForModule(roleName.Trim());

        //            // Obtener el rol recién creado
        //            var newRole = await _roleManager.FindByNameAsync(roleName.Trim());

        //            // Agregar reclamaciones para cada permiso generado
        //            foreach (var permission in permissions)
        //            {
        //                await _roleManager.AddClaimAsync(newRole, new Claim(CustomClaimTypes.Permission, permission));
        //            }

        //            // Configurar el mensaje de éxito
        //            TempData["success"] = $"Nuevo Rol: {roleName.Trim()}";
        //        }
        //        else
        //        {
        //            // Manejar error de creación de rol
        //            TempData["error"] = $"Error al crear el rol: {result.Errors.FirstOrDefault()?.Description ?? "Unknown error"}";
        //        }
        //    }
        //    else
        //    {
        //        // Manejar nombre de rol vacío
        //        TempData["error"] = "Nombre de rol no válido.";
        //    }

        //    // Redireccionar a la vista Index
        //    return RedirectToAction("Index");
        //}


        [HttpGet]
        public async Task<IActionResult> Edit(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }
           
            // Aquí puedes devolver una vista con un formulario de edición para el nombre y el nombre normalizado del rol.
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string roleId, string Name)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            // Guarda el nombre antiguo del rol.
            var oldName = role.Name;

            // Actualiza los nombres de los permisos asociados al rol.
            await UpdateSuperAdminPermissions(oldName, Name);
            await UpdatePermissionClaims(oldName, Name);

            // Esperar a que UpdateSuperAdminPermissions se complete antes de continuar
            await UpdateSuperAdminPermissions(oldName, Name);

            // Actualiza el nombre del rol con el nuevo valor.
            role.Name = Name;
            role.NormalizedName = _roleManager.NormalizeKey(Name);


            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["success"] = $"El Rol se actualizó correctamente";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = $"Se produjo un error al actualizar el rol";
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction("Index");
            }
        }


        private async Task UpdateSuperAdminPermissions(string oldRoleName, string newRoleName)
        {
            // Obtener el rol de superadministrador
            var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");

            // Verificar si el rol de superadministrador existe
            if (superAdminRole != null)
            {
                // Obtener las reclamaciones del rol de superadministrador
                var superAdminClaims = await _roleManager.GetClaimsAsync(superAdminRole);

                // Iterar sobre cada reclamación del rol de superadministrador
                foreach (var claim in superAdminClaims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}, Old Role Name: {oldRoleName}, New Role Name: {newRoleName}");

                    // Verificar si la reclamación es de tipo "Permission" y está asociada al rol antiguo
                    if (claim.Type == CustomClaimTypes.Permission && claim.Value.Contains($".{oldRoleName}."))
                    {
                        // Eliminar la reclamación asociada al rol antiguo
                        await _roleManager.RemoveClaimAsync(superAdminRole, claim);

                        // Crear una nueva reclamación asociada al rol nuevo con el mismo valor
                        await _roleManager.AddClaimAsync(superAdminRole, new Claim(CustomClaimTypes.Permission, claim.Value.Replace(oldRoleName, newRoleName)));
                    }
                }
            }
        }

        private async Task UpdatePermissionClaims(string oldRoleId, string newRoleName)
        {
              var oldRole = await _roleManager.FindByNameAsync(oldRoleId);                

                // Verificar si el rol nuevo existe
                if (oldRole != null)
                {
                    // Obtener las reclamaciones del rol antiguo
                    var oldRoleClaims = await _roleManager.GetClaimsAsync(oldRole);

                    // Iterar sobre cada reclamación del rol antiguo
                    foreach (var claim in oldRoleClaims)
                    {
                        if (claim.Type == CustomClaimTypes.Permission && claim.Value.Contains($".{oldRoleId}."))
                        {
                            // Eliminar la reclamación asociada al rol antiguo
                            await _roleManager.RemoveClaimAsync(oldRole, claim);

                            // Crear una nueva reclamación asociada al rol nuevo con el mismo valor
                            await _roleManager.AddClaimAsync(oldRole, new Claim(CustomClaimTypes.Permission, claim.Value.Replace(oldRoleId, newRoleName)));
                        }
                    }
                }
            
        }







        //public async Task<IActionResult> Edit(string roleId, string Name)
        //{
        //    var role = await _roleManager.FindByIdAsync(roleId);
        //    if (role == null)
        //    {
        //        return NotFound();
        //    }

        //    // Actualiza el nombre del rol con el nuevo valor.
        //    role.Name = Name;
        //    role.NormalizedName = _roleManager.NormalizeKey(Name);

        //    var result = await _roleManager.UpdateAsync(role);
        //    if (result.Succeeded)
        //    {
        //        TempData["success"] = $"El Rol se actualizó correctamente";
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        TempData["Error"] = $"Se produjo un error al actualizar el rol";
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //        return RedirectToAction("Index");
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            // Eliminar las asignaciones de usuarios a roles
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            foreach (var user in usersInRole)
            {
                await userManager.RemoveFromRoleAsync(user, role.Name);
            }

            // Eliminar las claims asociadas al rol
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in roleClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Eliminar el rol
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                // Eliminar las reclamaciones asociadas al rol de superadministrador
                await RemoveSuperAdminPermissions(role.Name);
                //TempData["success"] = $"El rol '{role.Name}' se eliminó correctamente.";
                return Ok(new { success = $"El rol '{role.Name}' se eliminó correctamente." });
            }
            else
            {
                //TempData["Error"] = $"Se produjo un error al eliminar el rol '{role.Name}'.";
                return BadRequest(new { error = $"Se produjo un error al eliminar el rol '{role.Name}'." });
            }

            
        }

        private async Task RemoveSuperAdminPermissions(string roleName)
        {
            // Obtener el rol de superadministrador
            var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");

            // Verificar si el rol de superadministrador existe
            if (superAdminRole != null)
            {
                // Obtener las reclamaciones del rol de superadministrador
                var superAdminClaims = await _roleManager.GetClaimsAsync(superAdminRole);

                // Iterar sobre cada reclamación del rol de superadministrador
                foreach (var claim in superAdminClaims)
                {
                    // Verificar si la reclamación está asociada al rol que se está eliminando
                    if (claim.Type == CustomClaimTypes.Permission && claim.Value.StartsWith($"Permissions.{roleName}."))
                    {
                        // Eliminar la reclamación asociada al rol
                        await _roleManager.RemoveClaimAsync(superAdminRole, claim);
                    }
                }
            }
        }





    }
}
