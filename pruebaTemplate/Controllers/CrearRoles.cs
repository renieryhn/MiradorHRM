using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlanillaPM.Models;
using PlanillaPM.Servicio;


namespace PlanillaPM.Controllers
{
    public class CrearRoles : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public CrearRoles(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [AllowAnonymous]
        public IActionResult AgregarRoles()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Authorize(Roles = Role.RolAdmin)]
        public async Task<IActionResult> AgregarRoles(TipoRol modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Verifica si el rol ya existe
            var existeRol = await roleManager.RoleExistsAsync(modelo.Name);

            if (!existeRol)
            {
                // Crea el rol si no existe
                var resultado = await roleManager.CreateAsync(new IdentityRole(modelo.Name));

                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Transacciones");
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "El rol ya existe.");
            }

            return View(modelo);
        }
    }

}
