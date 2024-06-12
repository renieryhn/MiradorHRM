
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PlanillaPM.Models;
using Syncfusion.EJ2.Diagrams;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                img = ConvertToBase64(e.FotografiaPath)
            }).ToList();

            // Agrega el valor en la primera posición
            nodes.Insert(0, new
            {
                id = 0,
                name = "Proyecto Mirador LLC",
                pid = "null",
                title = "main",
                img = ConvertToBase64("/img/MiEmpresa.jpeg")
            });

            ViewBag.nodes = nodes;

            return View(empleados);
        }

        private string ConvertToBase64(string imagePath)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

            if (!System.IO.File.Exists(path))
            {
                return string.Empty; // O maneja el caso donde la imagen no exista
            }

            var imageBytes = System.IO.File.ReadAllBytes(path);
            var base64String = Convert.ToBase64String(imageBytes);
            var imageFormat = Path.GetExtension(path).ToLower() == ".png" ? "png" : "jpeg"; // Asume que las imágenes son PNG o JPEG

            return $"data:image/{imageFormat};base64,{base64String}";
        }
    }

   
}

