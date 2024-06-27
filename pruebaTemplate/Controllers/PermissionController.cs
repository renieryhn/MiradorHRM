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

            var roles = await _roleManager.Roles.ToListAsync();
            var ventanas = await _context.Ventana.ToListAsync();
            var roleVentanas = await _context.RoleVentana
                .Where(rv => rv.RoleId == roleId)
                .ToListAsync();

            var model = new PermissionViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                Roles = roles.Select(r => new RoleViewModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    Selected = r.Id == roleId
                }).ToList(),
                Ventanas = ventanas,
                VentanasAsignadas = roleVentanas

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignInitialVentanas(string roleId, List<int> ventanaIds)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Invalid role ID.");
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            // Eliminar todas las asignaciones anteriores para este rol
            var existingRoleVentanas = _context.RoleVentana.Where(rv => rv.RoleId == roleId).ToList();
            _context.RoleVentana.RemoveRange(existingRoleVentanas);

            // Crear nuevas asignaciones para las ventanas seleccionadas
            foreach (var ventanaId in ventanaIds)
            {
                _context.RoleVentana.Add(new RoleVentana
                {
                    RoleId = roleId,
                    VentanaId = ventanaId,
                    Ver = true, // Valor predeterminado, ajustar según sea necesario
                    Crear = false,
                    Editar = false,
                    Eliminar = false
                });
            }

            await _context.SaveChangesAsync();

            return Ok(); // Retornar Ok para indicar que la operación fue exitosa
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVentanas(string roleId, int ventanaId, string permissionType, bool isSelected)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("Invalid role ID.");
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            var roleVentana = await _context.RoleVentana
                .FirstOrDefaultAsync(rv => rv.RoleId == roleId && rv.VentanaId == ventanaId);

            if (roleVentana == null)
            {
                return NotFound("RoleVentana not found.");
            }

            switch (permissionType)
            {
                case "ver":
                    roleVentana.Ver = isSelected;
                    break;
                case "crear":
                    roleVentana.Crear = isSelected;
                    break;
                case "editar":
                    roleVentana.Editar = isSelected;
                    break;
                case "eliminar":
                    roleVentana.Eliminar = isSelected;
                    break;
                default:
                    return BadRequest("Invalid permission type.");
            }

            _context.RoleVentana.Update(roleVentana);
            await _context.SaveChangesAsync();

            return Ok(); // Return Ok to indicate successful operation
        }







    }

}
