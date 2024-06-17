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
using PlanillaPM.Seeds;

namespace PlanillaPM.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly PlanillaContext _context;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<Usuario> userManager, PlanillaContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        private const string PermissionClaimType = "Permission";

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
                    // Obtener el rol recién creado
                    var newRole = await _roleManager.FindByNameAsync(roleName.Trim());

                    // Agregar solo la reclamación de "View" al nuevo rol
                    await _roleManager.AddClaimAsync(newRole, new Claim(roleName, "View"));

                    // Obtener el rol de superadministrador
                    //var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");

                    //// Asignar todos los permisos al rol de superadministrador
                    //if (superAdminRole != null)
                    //{
                    //    var allPermissions = Permissions.GeneratePermissionsForModule();
                    //    foreach (var permission in allPermissions)
                    //    {
                    //        await _roleManager.AddClaimAsync(superAdminRole, new Claim(permission, permission));
                    //    }
                    //}

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
            //await UpdateSuperAdminPermissions(oldName, Name);
            await UpdatePermissionClaims(oldName, Name);

            // Esperar a que UpdateSuperAdminPermissions se complete antes de continuar
            //await UpdateSuperAdminPermissions(oldName, Name);

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
                    if (claim.Type == oldRoleId)
                    {
                        // Eliminar la reclamación asociada al rol antiguo
                        await _roleManager.RemoveClaimAsync(oldRole, claim);

                        // Crear una nueva reclamación asociada al rol nuevo con el mismo valor
                        await _roleManager.AddClaimAsync(oldRole, new Claim(newRoleName, claim.Value));
                    }
                }
            }
        }

        //private async Task UpdateSuperAdminPermissions(string oldRoleName, string newRoleName)
        //{
        //    // Obtener el rol de superadministrador
        //    var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");

        //    // Verificar si el rol de superadministrador existe
        //    if (superAdminRole != null)
        //    {
        //        // Obtener las reclamaciones del rol de superadministrador
        //        var superAdminClaims = await _roleManager.GetClaimsAsync(superAdminRole);

        //        // Iterar sobre cada reclamación del rol de superadministrador
        //        foreach (var claim in superAdminClaims)
        //        {
        //            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}, Old Role Name: {oldRoleName}, New Role Name: {newRoleName}");

        //            // Verificar si la reclamación es de tipo antiguo
        //            if (claim.Type == oldRoleName)
        //            {
        //                // Eliminar la reclamación asociada al rol antiguo
        //                await _roleManager.RemoveClaimAsync(superAdminRole, claim);

        //                // Crear una nueva reclamación asociada al rol nuevo con el mismo valor
        //                await _roleManager.AddClaimAsync(superAdminRole, new Claim(newRoleName, claim.Value));
        //            }
        //        }
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
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            foreach (var user in usersInRole)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                if (!result.Succeeded)
                {
                    // Manejar errores al eliminar usuarios de roles
                    return BadRequest(new { error = $"Error al eliminar el usuario {user.UserName} del rol {role.Name}." });
                }
            }
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in roleClaims)
            {
                var result = await _roleManager.RemoveClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    // Manejar errores al eliminar reclamos
                    return BadRequest(new { error = $"Error al eliminar la reclamación {claim.Type} del rol {role.Name}." });
                }
            }

            // Eliminar el rol
            var deleteResult = await _roleManager.DeleteAsync(role);
            if (deleteResult.Succeeded)
            {
                // Eliminar las reclamaciones asociadas al rol de superadministrador
                //await RemoveSuperAdminPermissions(role.Name);
                return Ok(new { success = $"El rol '{role.Name}' se eliminó correctamente." });
            }
            else
            {
                return BadRequest(new { error = $"Se produjo un error al eliminar el rol '{role.Name}'." });
            }
        }

        //private async Task RemoveSuperAdminPermissions(string roleName)
        //{
        //    // Obtener el rol de superadministrador
        //    var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");

        //    // Verificar si el rol de superadministrador existe
        //    if (superAdminRole != null)
        //    {
        //        // Obtener las reclamaciones del rol de superadministrador
        //        var superAdminClaims = await _roleManager.GetClaimsAsync(superAdminRole);

        //        // Iterar sobre cada reclamación del rol de superadministrador
        //        foreach (var claim in superAdminClaims)
        //        {
        //            // Verificar si la reclamación está asociada al rol que se está eliminando
        //            if (claim.Type == roleName)
        //            {
        //                // Eliminar la reclamación asociada al rol
        //                await _roleManager.RemoveClaimAsync(superAdminRole, claim);
        //            }
        //        }
        //    }
        //}

    }
}
