using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlanillaPM.ViewModel;
using pruebaTemplate.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity.UI.Services;
using PlanillaPM.Models;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Syncfusion.Pdf;
using Syncfusion.DocIORenderer;
using Syncfusion.HtmlConverter;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2010.Excel;
using static PlanillaPM.Models.EmpleadoAusencium;



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
            var empleadoAusencias = await AusenciasTomarAccion();
            var totalHombres = await _context.Empleados.CountAsync(e => e.Genero == "Masculino");
            var totalMujeres = await _context.Empleados.CountAsync(e => e.Genero == "Femenino");

            InicioFiltros viewModel=null;
            var user = await _userManager.GetUserAsync(User);
            try
            {
                viewModel = new InicioFiltros
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
                    ContratosPorVencer = contratosPorVencer,
                    EmpleadoAusencias = empleadoAusencias,
                    TotalHombres = totalHombres,
                    TotalMujeres = totalMujeres
                };
                if (viewModel == null)
                {
                    return NotFound();
                } else
                {
                  //  var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                    //var IdTipoContratoNavigation = await _context.EmpleadoContratos.ToListAsync();
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return NotFound();
            }
        }

        private async Task<List<Empleado>> ObtenerProximosCumpleañeros()
        {
            int id = 0;
            List<Empleado> proximosCumpleañeros = _context.Empleados.Where(e => e.IdEmpleado == id).ToList();

            // Obtener la fecha de hoy
            DateTime hoy = DateTime.Today;

            // Calcular la fecha de hace 7 días
            DateTime haceVeinteDias = hoy.AddDays(20);

            // Obtener los próximos cumpleañeros activos en los próximos 7 días
            // Obtener los próximos cumpleañeros activos en los próximos 20 días
            try
            {
                proximosCumpleañeros = await _context.Empleados
                .Where(e => e.Activo &&
                    (e.FechaNacimiento.DayOfYear >= hoy.DayOfYear && e.FechaNacimiento.DayOfYear <= haceVeinteDias.DayOfYear))
                .OrderBy(e => e.FechaNacimiento)
                .Take(10)
                .ToListAsync();
                return proximosCumpleañeros;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return proximosCumpleañeros;
            }
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
            int id = 0;
            List<Empleado> licenciasPorVencer = _context.Empleados
                .Include(e => e.IdClaseEmpleadoNavigation)               
                .Where(e => e.IdEmpleado == id).ToList();
            try
            {
                licenciasPorVencer = await _context.Empleados
                .Where(e => e.Activo &&
                    e.FechaVencimientoLicencia >= hoyDateOnly && e.FechaVencimientoLicencia <= haceVeinteDiasDateOnly)
                .OrderBy(e => e.FechaVencimientoLicencia)
                .ToListAsync();
                if (licenciasPorVencer == null)
                {
                    return licenciasPorVencer;
                }
                else
                {
                    return licenciasPorVencer;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return licenciasPorVencer;
            }
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
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdTipoContratoNavigation)
                .Where(e => e.Activo &&
                    e.FechaFin >= hoyDateOnly && e.FechaFin <= haceCincuentaDiasDateOnly)
                .OrderBy(e => e.FechaFin)
                .ToListAsync();
            if(contratosPorVencer == null)
            {
                return null;
            }
            else
            {
                return contratosPorVencer;
            }
        }

        private async Task<List<EmpleadoAusencium>> AusenciasTomarAccion()
        {
            var empleadoAusencias = await _context.EmpleadoAusencia
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdTipoAusenciaNavigation)
                .Where(e => e.Activo && e.Estado == EstadoAusencia.Solicitada)
                .ToListAsync();

            return empleadoAusencias;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEstado([FromBody] InicioFiltros request)
        {
            var empleadoAusencia = await _context.EmpleadoAusencia.FindAsync(request.Id);
            if (empleadoAusencia == null)
            {
                return Json(new { success = false, message = "Ausencia no encontrada." });
            }

            // Acceder al usuario actual
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado." });
            }

            // Actualizar el estado y el usuario que aprueba la ausencia
            empleadoAusencia.Estado = request.NuevoEstado;
            empleadoAusencia.AprobadoPor = usuarioActual.Email; // Asignar directamente el usuario actual

            // Establecer campos de auditoría
            SetCamposAuditoria(empleadoAusencia, false); // false indica que no es un nuevo registro

            _context.Update(empleadoAusencia);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Estado actualizado correctamente." });
        }

        private void SetCamposAuditoria(EmpleadoAusencium record, bool bNewRecord)
        {
            var now = DateTime.Now;
            var CurrentUser = _userManager.GetUserName(User);

            if (bNewRecord)
            {
                record.FechaCreacion = now;
                record.CreadoPor = CurrentUser;
                record.FechaModificacion = now;
                record.ModificadoPor = CurrentUser;
                record.Activo = true;
            }
            else
            {
                record.FechaModificacion = now;
                record.ModificadoPor = CurrentUser;
            }
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ActualizarEstado([FromBody] InicioFiltros request)
        //{
        //    var empleadoAusencia = await _context.EmpleadoAusencia.FindAsync(request.Id);
        //    if (empleadoAusencia == null)
        //    {
        //        return Json(new { success = false, message = "Ausencia no encontrada." });
        //    }

        //    empleadoAusencia.Estado = request.NuevoEstado;
        //    _context.Update(empleadoAusencia);
        //    await _context.SaveChangesAsync();

        //    return Json(new { success = true, message = "Estado actualizado correctamente." });
        //}


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
