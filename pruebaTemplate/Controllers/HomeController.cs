using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlanillaPM.ViewModel;
using pruebaTemplate.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using PlanillaPM.Models;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using Syncfusion.Pdf;
using Syncfusion.DocIORenderer;
using Syncfusion.HtmlConverter;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;



namespace pruebaTemplate.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Usuario> _userManager;
        private readonly PlanillaContext _context;

        public HomeController(ILogger<HomeController> logger, Microsoft.AspNetCore.Identity.UserManager<Usuario> userManager, PlanillaContext context)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {          

            // Consulta para contar la cantidad de empleados
            var cantidadEmpleados = await _context.Empleados.Where(e => e.Activo).CountAsync();
            var cantidadUsuarios = await _userManager.Users.CountAsync();
            var cantidadCargos = await _context.Cargos.CountAsync();
            int cantidadPerfilesCompletos = await _context.Empleados
            .Where(e => e.Activo
             && !string.IsNullOrEmpty(e.NombreEmpleado)
             && !string.IsNullOrEmpty(e.ApellidoEmpleado)
             && e.FechaNacimiento != null
             && !string.IsNullOrEmpty(e.Email)
             && !string.IsNullOrEmpty(e.NumeroIdentidad)
             //&& !string.IsNullOrEmpty(e.NumeroLicencia)
             //&& e.FechaVencimientoLicencia != null
             && !string.IsNullOrEmpty(e.Genero)
             //&& e.Fotografia != null
             && !string.IsNullOrEmpty(e.Direccion)
             && !string.IsNullOrEmpty(e.Telefono)
             && !string.IsNullOrEmpty(e.CiudadResidencia)
             //&& e.IdCargo != null
             //&& e.IdDepartamento != null
             //&& e.IdTipoContrato != null
             //&& e.IdTipoNomina != null
             //&& e.IdEncargado != null
             //&& e.IdClaseEmpleado != null
             //&& e.EstadoCivil != null
             //&& e.FechaInicio != null
             //&& e.IdBanco != null
             //&& !string.IsNullOrEmpty(e.CuentaBancaria)
             //&& !string.IsNullOrEmpty(e.NumeroRegistroTributario)
             && e.SalarioBase != null)
            //&& !string.IsNullOrEmpty(e.Comentarios)
            //&& !string.IsNullOrEmpty(e.Observaciones)
            //&& e.FechaInactivacion == null
            //&& string.IsNullOrEmpty(e.MotivoInactivacion))
            .CountAsync();

            var proximosCumpleañeros = await ObtenerProximosCumpleañeros();
            var licenciasPorVencer = await ObtenerLicenciasPorVencer();
            var contratosPorVencer = await ObtenerContratosPorVencer();

            var user = await _userManager.GetUserAsync(User);

            var viewModel = new InicioFiltros
            {
                Profile = new ProfileViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                   
                },
                CantidadEmpleados = cantidadEmpleados,
                CantidadUsuarios = cantidadUsuarios,
                CantidadPerfilesCompletos = cantidadPerfilesCompletos,
                CantidadCargos = cantidadCargos,
                ProximosCumpleañeros = proximosCumpleañeros,
                LicenciasPorVencer = licenciasPorVencer,
                ContratosPorVencer = contratosPorVencer
            };

            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            var IdTipoContratoNavigation = await _context.EmpleadoContratos.ToListAsync();


            return View(viewModel);
        }

        private async Task<List<Empleado>> ObtenerProximosCumpleañeros()
        {
            // Obtener la fecha de hoy
            DateTime hoy = DateTime.Today;

            // Calcular la fecha de hace 7 días
            DateTime haceVeinteDias = hoy.AddDays(20);

            // Obtener los próximos cumpleañeros activos en los próximos 7 días
            // Obtener los próximos cumpleañeros activos en los próximos 20 días
            var proximosCumpleañeros = await _context.Empleados
                .Where(e => e.Activo &&
                    (e.FechaNacimiento.DayOfYear >= hoy.DayOfYear && e.FechaNacimiento.DayOfYear <= haceVeinteDias.DayOfYear))
                .OrderBy(e => e.FechaNacimiento)
                .Take(10)
                .ToListAsync();


            return proximosCumpleañeros;
        }

        private async Task<List<Empleado>> ObtenerLicenciasPorVencer()
        {
            // Obtener la fecha de hoy
            DateTime hoy = DateTime.Today;

            // Calcular la fecha de hace 20 días
            DateTime haceVeinteDias = hoy.AddDays(50);

            // Convertir hoy a DateOnly
            var hoyDateOnly = DateOnly.FromDateTime(DateTime.Today);

            // Convertir haceVeinteDias a DateOnly
            var haceVeinteDiasDateOnly = DateOnly.FromDateTime(haceVeinteDias);

            var licenciasPorVencer = await _context.Empleados
                .Where(e => e.Activo &&
                    e.FechaVencimientoLicencia >= hoyDateOnly && e.FechaVencimientoLicencia <= haceVeinteDiasDateOnly)
                .OrderBy(e => e.FechaVencimientoLicencia)
                .ToListAsync();

            return licenciasPorVencer;
        }

        private async Task<List<EmpleadoContrato>> ObtenerContratosPorVencer()
        {
            // Obtener la fecha de hoy
            DateTime hoy = DateTime.Today;

            // Calcular la fecha de hace 50 días
            DateTime haceCincuentaDias = hoy.AddDays(50);

            // Convertir hoy a DateOnly
            var hoyDateOnly = DateOnly.FromDateTime(DateTime.Today);

            // Convertir haceCincuentaDias a DateOnly
            var haceCincuentaDiasDateOnly = DateOnly.FromDateTime(haceCincuentaDias);

            var contratosPorVencer = await _context.EmpleadoContratos
                .Where(e => e.Activo &&
                    e.FechaFin >= hoyDateOnly && e.FechaFin <= haceCincuentaDiasDateOnly)
                .OrderBy(e => e.FechaFin)
                .ToListAsync();

            return contratosPorVencer;
        }



        public IActionResult Privacy()
        {
            return View();
        }

      


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

   

       


    }
}
