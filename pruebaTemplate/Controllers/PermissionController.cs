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
            // Verificar si el ID del rol es nulo o vacío
            if (string.IsNullOrEmpty(roleId))
            {
                return NotFound(); // Devolver una respuesta NotFound si el ID del rol es nulo o vacío
            }

            // Obtener el rol
            var role = await _roleManager.FindByIdAsync(roleId);

            // Verificar si el rol existe
            if (role != null)
            {
                // Obtener todas las reclamaciones asociadas al rol desde la base de datos
                var claims = await _roleManager.GetClaimsAsync(role);

                // Obtener todos los permisos disponibles en la base de datos
                var allClaims = await _context.AspNetRoleClaims
                    .Where(c => c.ClaimType == CustomClaimTypes.Permission)
                    .Select(c => c.ClaimValue)
                    .ToListAsync();

                // Obtener los permisos asignados al rol
                var assignedClaims = claims.Select(c => c.Value).ToList();

                // Obtener los permisos no asignados al rol
                var unassignedClaims = allClaims.Where(c => !assignedClaims.Contains(c)).ToList();

                // Convertir las reclamaciones en el formato adecuado para la vista
                var model = new PermissionViewModel
                {
                    RoleId = roleId,
                    RoleClaims = assignedClaims.Select(claim => new RoleClaimsViewModel
                    {
                        Value = claim,
                        Selected = true // Establecer Selected como true ya que está asignado al rol
                    }).ToList(),
                    UnassignedPermissions = unassignedClaims.Select(claim => new RoleClaimsViewModel
                    {
                        Value = claim,
                        Selected = false // Establecer Selected como false ya que no está asignado al rol
                    }).ToList()
                };

                return View(model);
            }
            else
            {
                return NotFound(); // Devolver una respuesta NotFound si el rol no fue encontrado
            }
        }



        public async Task<IActionResult> Update(PermissionViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound(); // Devolver una respuesta NotFound si el rol no fue encontrado
            }

            // Obtener los permisos seleccionados para agregar
            var selectedClaims = model.UnassignedPermissions.Where(a => a.Selected).Select(a => a.Value).ToList();

            // Agregar los permisos seleccionados al rol
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
                return NotFound(); // Devolver una respuesta NotFound si el rol no fue encontrado
            }

            // Obtener los permisos desmarcados para eliminar
            var unselectedClaims = model.RoleClaims.Where(a => !a.Selected).Select(a => a.Value).ToList();

            // Eliminar los permisos desmarcados del rol
            foreach (var claimValue in unselectedClaims)
            {
                await _roleManager.RemovePermissionClaim(role, claimValue);
            }

            return RedirectToAction("Index", new { roleId = model.RoleId });
        }



    }
}
