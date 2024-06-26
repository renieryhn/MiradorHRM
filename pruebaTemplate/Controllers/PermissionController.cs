using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiradorHRM.Models;
using PlanillaPM.Constants;
using PlanillaPM.Helpers;
using PlanillaPM.Models;
using System.Security.Claims;
using System.Threading.Tasks;



namespace PlanillaPM.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class PermissionController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PlanillaContext _context;

        public PermissionController(RoleManager<IdentityRole> roleManager, PlanillaContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }


        public async Task<IActionResult> GetPermissions(string selectedRoleId)
        {
            if (string.IsNullOrEmpty(selectedRoleId))
            {
                return RedirectToAction("Index");
            }

            var role = await _roleManager.FindByIdAsync(selectedRoleId);
            if (role == null)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", new { roleId = selectedRoleId });
        }



        public async Task<IActionResult> Index(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }


            var allPermissions = Permissions.GeneratePermissionsForModule();
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            var assignedPermissions = roleClaims
                .Where(c => c.Type == role.Name)
                .Select(c => c.Value)
                .ToList();


            var roles = await _roleManager.Roles.ToListAsync();
            var ventanas = _context.Ventana.ToList();
            var ventanasAsignadas = await _context.RoleVentana
               .Where(rv => rv.RoleId == roleId)
               .Select(rv => rv.VentanaId)
               .ToListAsync();

            var model = new PermissionViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                AssignedPermissions = allPermissions.Where(p => assignedPermissions.Contains(p))
                    .Select(permission => new RoleClaimsViewModel
                    {
                        Type = role.Name,
                        Value = permission,
                        Selected = true
                    }).ToList(),
                UnassignedPermissions = allPermissions.Where(p => !assignedPermissions.Contains(p))
                    .Select(permission => new RoleClaimsViewModel
                    {
                        Type = role.Name,
                        Value = permission,
                        Selected = false
                    }).ToList(),

                Roles = roles.Select(r => new RoleViewModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    Selected = r.Id == roleId
                }).ToList()

            };

            cGeneralFun cGen = new cGeneralFun();

            
           
            ViewBag.Ventanas = ventanas;
            ViewBag.VentanasAsignadas = ventanasAsignadas;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignVentanas(string roleId, List<int> ventanaIds)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Invalid role ID.");
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            var existingRoleVentanas = _context.RoleVentana.Where(rv => rv.RoleId == roleId).ToList();

            // Eliminar ventanas que ya no están seleccionadas
            var ventanasToRemove = existingRoleVentanas
                .Where(rv => !ventanaIds.Contains(rv.VentanaId))
                .ToList();
            _context.RoleVentana.RemoveRange(ventanasToRemove);

            // Agregar nuevas ventanas seleccionadas
            var ventanasToAdd = ventanaIds
                .Where(ventanaId => !existingRoleVentanas.Any(rv => rv.VentanaId == ventanaId))
                .Select(ventanaId => new RoleVentana
                {
                    RoleId = roleId,
                    VentanaId = ventanaId
                }).ToList();
            _context.RoleVentana.AddRange(ventanasToAdd);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { roleId });
        }




        [HttpPost]
        public async Task<IActionResult> Update(PermissionViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.RoleId))
            {
                return BadRequest("Invalid role ID.");
            }

            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound();
            }


            var roleClaims = await _roleManager.GetClaimsAsync(role);

            if (model.UnassignedPermissions != null)
            {
                // Asignar permisos seleccionados
                var selectedPermissions = model.UnassignedPermissions
                                                .Where(p => p.Selected)
                                                .Select(p => p.Value)
                                                .ToList();

                foreach (var permission in selectedPermissions)
                {
                    if (!roleClaims.Any(c => c.Type == model.RoleName && c.Value == permission))
                    {

                        var result = await _roleManager.AddClaimAsync(role, new Claim(model.RoleName, permission));
                        if (!result.Succeeded)
                        {
                            // Manejar el error, tal vez registrarlo o devolver un mensaje de error
                            ModelState.AddModelError("", $"Error adding permission {permission} to role {model.RoleName}");
                        }
                    }
                }
            }

            //return RedirectToAction("Index", new { roleId = model.RoleId });
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemovePermissions(PermissionViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.RoleId))
            {
                return BadRequest("Invalid role ID.");
            }

            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound();
            }

            var roleClaims = await _roleManager.GetClaimsAsync(role);

            if (model.AssignedPermissions != null)
            {
                // Remover permisos deseleccionados
                var unselectedPermissions = model.AssignedPermissions
                                                 .Where(p => !p.Selected)
                                                 .Select(p => p.Value)
                                                 .ToList();

                foreach (var permission in unselectedPermissions)
                {
                    // Encontrar la claim correspondiente en roleClaims
                    var claim = roleClaims.FirstOrDefault(c => c.Type == model.RoleName && c.Value == permission);
                    if (claim != null)
                    {
                        var result = await _roleManager.RemoveClaimAsync(role, claim);
                        if (!result.Succeeded)
                        {
                            // Manejar el error, tal vez registrarlo o devolver un mensaje de error
                            ModelState.AddModelError("", $"Error removing permission {permission} from role {model.RoleName}");
                        }
                    }
                }
            }

            //return RedirectToAction("Index", new { roleId = model.RoleId });
            return Ok();
        }


    }

}
