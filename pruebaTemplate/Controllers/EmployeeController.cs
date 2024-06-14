using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using Syncfusion.EJ2.Diagrams;




namespace PlanillaPM.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmployeeController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var empleados = await _context.Empleados
                .Include(e => e.IdEncargadoNavigation)
                .Include(e => e.IdCargoNavigation)
                .ToListAsync();

            ViewBag.nodes = empleados.Select(e => new {
                IdEmpleado = e.IdEmpleado,
                NombreCompleto = e.NombreCompleto,
                Cargo = e.IdCargoNavigation.NombreCargo,
                IdEncargado = e.IdEncargado.HasValue ? e.IdEncargadoNavigation.IdEmpleado.ToString() : null,
                Image = e.FotografiaPath
            }).ToList();

            return View(empleados);
        }

        public async Task<IActionResult> Index2()
        {
            var empleados = await _context.Empleados
                .Include(e => e.IdEncargadoNavigation)
                .Include(e => e.IdCargoNavigation)
                .ToListAsync();

            var nodes = empleados.Select(e => new
            {
                id = e.IdEmpleado,
                name = e.NombreCompleto,
                pid = e.IdEncargado.HasValue ? e.IdEncargadoNavigation.IdEmpleado.ToString() : "0",
                title = e.IdCargoNavigation.NombreCargo,
                img = e.FotografiaPath
            }).ToList();

            // Agrega el valor en la primera posición
            nodes.Insert(0, new
            {
                id = 0,
                name = "Proyecto Mirador LLC",
                pid = "null",
                title = "main",
                img = "/img/MiEmpresa.jpeg"
            });

            ViewBag.nodes = nodes;

            return View(empleados);
        }

    }
}

