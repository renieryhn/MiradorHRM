using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Constants;
using PlanillaPM.Helpers;
using PlanillaPM.Models;
using System.Security.Claims;
using System.Threading.Tasks;



namespace PlanillaPM.Controllers
{
    ///[Authorize(Roles = "SuperAdmin")]
    public class PermissionController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PlanillaContext _context;

        public PermissionController(RoleManager<IdentityRole> roleManager, PlanillaContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return NotFound(); // Devuelve NotFound si el ID del rol es nulo o vacío
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound(); // Devuelve NotFound si no se encuentra el rol
            }

            // Obtener todos los permisos disponibles para el módulo
            var allPermissions = Permissions.GeneratePermissionsForModule();

            // Obtener los permisos asignados al rol
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            var assignedClaims = roleClaims
                .Where(c => c.Type == role.Name && allPermissions.Contains(c.Value))
                .Select(c => c.Value)
                .ToList();

            // Filtrar los permisos no asignados
            var unassignedPermissions = allPermissions.Except(assignedClaims).ToList();

            // Crear el modelo para la vista
            var model = new PermissionViewModel
            {
                RoleId = role.Name,
                RoleClaims = assignedClaims.Select(claim => new RoleClaimsViewModel
                {
                    Value = claim,
                    Selected = true
                }).ToList(),
                UnassignedPermissions = unassignedPermissions.Select(claim => new RoleClaimsViewModel
                {
                    Value = claim,
                    Selected = false
                }).ToList()
            };

            return View(model);
        }


        public async Task<IActionResult> Update(PermissionViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound();
            }

            var selectedClaims = model.UnassignedPermissions.Where(a => a.Selected).Select(a => a.Value).ToList();
            foreach (var claimValue in selectedClaims)
            {
                await _roleManager.AddPermissionClaim(role, claimValue);
            }

            return RedirectToAction("Index", new { roleId = model.RoleId });
        }

        public async Task<IActionResult> RemovePermissions(PermissionViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound();
            }

            var unselectedClaims = model.RoleClaims.Where(a => !a.Selected).Select(a => a.Value).ToList();
            foreach (var claimValue in unselectedClaims)
            {
                await _roleManager.RemovePermissionClaim(role, claimValue);
            }

            return RedirectToAction("Index", new { roleId = model.RoleId });
        }
    }

}
