using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
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

        public async Task<IActionResult> Index(int pg, string roleId)
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

            // Agregar paginación
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = model.VentanasAsignadas.Count(); // Ajustar según la colección que deseas paginar
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = model.VentanasAsignadas.Skip(recSkip).Take(pager.PageSize).ToList(); // Ajustar según la colección que deseas paginar
            this.ViewBag.Pager = pager;
            this.ViewBag.RoleId = roleId; // Pasar roleId a la vista

            model.VentanasAsignadas = data; // Actualizar el modelo con los datos paginados

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


        [HttpPost]
        public async Task<IActionResult> CloneRole(string originalRoleId, string newRoleName)
        {
            // Verificar que el nombre del nuevo rol no esté vacío
            if (string.IsNullOrEmpty(newRoleName))
            {
                return BadRequest("El nombre del nuevo rol no puede estar vacío.");
            }

            // Buscar el rol original por su Id
            var originalRole = await _roleManager.FindByIdAsync(originalRoleId);
            if (originalRole == null)
            {
                return NotFound();
            }

            try
            {
                // Crear el nuevo rol basado en el original
                var newRole = new IdentityRole(newRoleName.Trim());

                // Intentar crear el nuevo rol
                var result = await _roleManager.CreateAsync(newRole);
                if (!result.Succeeded)
                {
                    // Manejar errores si la creación del rol falla
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest($"Error al crear el nuevo rol: {errors}");
                }

                // Obtener el ID del nuevo rol creado
                var newRoleId = newRole.Id;

                // Obtener los permisos del rol original
                var originalPermissions = _context.RoleVentana.Where(rv => rv.RoleId == originalRoleId).ToList();

                // Copiar los permisos al nuevo rol
                foreach (var permission in originalPermissions)
                {
                    var newPermission = new RoleVentana
                    {
                        RoleId = newRoleId,
                        VentanaId = permission.VentanaId,
                        Ver = permission.Ver,
                        Crear = permission.Crear,
                        Editar = permission.Editar,
                        Eliminar = permission.Eliminar
                    };
                    _context.RoleVentana.Add(newPermission);
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok(); // Operación exitosa
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }





    }

}
