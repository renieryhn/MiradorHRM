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
using MiradorHRM.Models;
using static PlanillaPM.Models.VacacionDetalle;
using DocumentFormat.OpenXml.Spreadsheet;
using static PlanillaPM.Models.Viatico;
using Microsoft.Win32;




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

    public IActionResult Bienvenido()
    {
        return View();
    }

    public async Task<IActionResult> IndexAsync(int pg)
        {
        
        var userName = User.Identity?.Name;     
        var user = await _userManager.FindByNameAsync(userName);     
        var roles = await _userManager.GetRolesAsync(user);
        if (roles == null || !roles.Any())
        {
          
            return RedirectToAction("Bienvenido");
        }


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
            var empleadoDepartamento = await EmpleadoDepartamento();


        var totalHombres = await _context.Empleados.CountAsync(e => e.Genero == "Masculino");
            var totalMujeres = await _context.Empleados.CountAsync(e => e.Genero == "Femenino");

            var empleados = await _context.Empleados.Include(e => e.IdUbicacionNavigation).ToListAsync();
            var empleadosPorUbicacion = empleados
                .Where(e => e.IdUbicacionNavigation != null)
                .GroupBy(e => e.IdUbicacionNavigation.NombreUbicacion)
                .Select(group => new
                {
                    Ubicacion = group.Key,
                    TotalEmpleados = group.Count()
                })
                .ToDictionary(x => x.Ubicacion, x => x.TotalEmpleados);

            try
            {
               
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
                    ContratosPorVencer = contratosPorVencer,
                    EmpleadoAusencias = empleadoAusencias,
                    EmpleadoDepartamento = empleadoDepartamento,
                    TotalHombres = totalHombres,
                    TotalMujeres = totalMujeres,
                    EmpleadosPorUbicacion = empleadosPorUbicacion
                    
                };

            // Paginación para ProximosCumpleañeros
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCountCumple = viewModel.ProximosCumpleañeros.Count();
            var pagerCumple = new Pager(recsCountCumple, pg, pageSize);
            int recSkipCumple = (pg - 1) * pageSize;
            viewModel.ProximosCumpleañeros = viewModel.ProximosCumpleañeros
                .Skip(recSkipCumple)
                .Take(pagerCumple.PageSize)
                .ToList();
            this.ViewBag.PagerCumple = pagerCumple;

            // Paginación para LicenciasPorVencer
            int recsCountLicencias = viewModel.LicenciasPorVencer.Count();
            var pagerLicencias = new Pager(recsCountLicencias, pg, pageSize);
            int recSkipLicencias = (pg - 1) * pageSize;
            viewModel.LicenciasPorVencer = viewModel.LicenciasPorVencer
                .Skip(recSkipLicencias)
                .Take(pagerLicencias.PageSize)
                .ToList();
            this.ViewBag.PagerLicencias = pagerLicencias;

            // Paginación para ContratosPorVencer
            int recsCountContratos = viewModel.ContratosPorVencer.Count();
            var pagerContratos = new Pager(recsCountContratos, pg, pageSize);
            int recSkipContratos = (pg - 1) * pageSize;
            viewModel.ContratosPorVencer = viewModel.ContratosPorVencer
                .Skip(recSkipContratos)
                .Take(pagerContratos.PageSize)
                .ToList();
            this.ViewBag.PagerContratos = pagerContratos;

            // Paginación para EmpleadoAusencias
            int recsCountAusencias = viewModel.EmpleadoAusencias.Count();
            var pagerAusencias = new Pager(recsCountAusencias, pg, pageSize);
            int recSkipAusencias = (pg - 1) * pageSize;
            viewModel.EmpleadoAusencias = viewModel.EmpleadoAusencias
                .Skip(recSkipAusencias)
                .Take(pagerAusencias.PageSize)
                .ToList();
            this.ViewBag.PagerAusencias = pagerAusencias;

            return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return NotFound();
            }
        }

        public async Task<IActionResult> DashboardNomina(int pg)
        {

            var nominasData = await _context.Nominas
           .GroupBy(n => n.Mes)
           .Select(g => new NominaDataCharts
           {
               Mes = g.Key,
               TotalIngresosCharts = (decimal)g.Sum(n => n.TotalIngresos),
               TotalDeduccionesCharts = (decimal)g.Sum(n => n.TotalDeducciones),
               TotalImpuestosCharts = (decimal)g.Sum(n => n.TotalImpuestos),
               PagoNetoCharts = (decimal)g.Sum(n => n.PagoNeto)
           }).ToListAsync();

            ViewBag.NominasData = nominasData;

            var totales = await ObtenerYCalcularTotalesAsync();
            var solicituddetalle = await SolicitudTomarAccion();
            var nomina = await NominaTomarAccion();
            var viatico = await SolicitudViaticoTomarAccion();


            

        var viewModel = new NominaTotales
            {
                TotalIngresos = totales.TotalIngresos,
                TotalDeducciones = totales.TotalDeducciones,
                TotalImpuestos = totales.TotalImpuestos,
                TotalEmpleadosEnNomina = totales.TotalEmpleadosEnNomina,
                VacacionDetalle = solicituddetalle,
                NominaAprovacion = nomina,
                Viatico=viatico
        };

        // Paginación para VacacionDetalle
        const int pageSize = 10;
        if (pg < 1) pg = 1;

        // Paginación para VacacionDetalle
        int recsCountVacacionDetalle = viewModel.VacacionDetalle.Count();
        var pagerVacacionDetalle = new Pager(recsCountVacacionDetalle, pg, pageSize);
        int recSkipVacacionDetalle = (pg - 1) * pageSize;
        viewModel.VacacionDetalle = viewModel.VacacionDetalle
            .Skip(recSkipVacacionDetalle)
            .Take(pagerVacacionDetalle.PageSize)
            .ToList();
        this.ViewBag.PagerVacacionDetalle = pagerVacacionDetalle;

        // Paginación para NominaAprovacion
        int recsCountNominaAprovacion = viewModel.NominaAprovacion.Count();
        var pagerNominaAprovacion = new Pager(recsCountNominaAprovacion, pg, pageSize);
        int recSkipNominaAprovacion = (pg - 1) * pageSize;
        viewModel.NominaAprovacion = viewModel.NominaAprovacion
            .Skip(recSkipNominaAprovacion)
            .Take(pagerNominaAprovacion.PageSize)
            .ToList();
        this.ViewBag.PagerNominaAprovacion = pagerNominaAprovacion;

        // Paginación para Viatico
        int recsCountViatico = viewModel.Viatico.Count();
        var pagerViatico = new Pager(recsCountViatico, pg, pageSize);
        int recSkipViatico = (pg - 1) * pageSize;
        viewModel.Viatico = viewModel.Viatico
            .Skip(recSkipViatico)
            .Take(pagerViatico.PageSize)
            .ToList();
        this.ViewBag.PagerViatico = pagerViatico;


        return View(viewModel);


        }

        //Dahsboard nomina
        public async Task<NominaTotales> ObtenerYCalcularTotalesAsync()
        {
            var fechaActual = DateTime.Now;
            int añoActual = fechaActual.Year;
            int mesActual = fechaActual.Month;

            var nominasDelMesActual = await _context.Nominas
                .Where(n => n.PeriodoFiscal == añoActual && n.Mes == mesActual)
                .ToListAsync();

            var totales = new NominaTotales
            {
                TotalIngresos = nominasDelMesActual.Any() ? (decimal)nominasDelMesActual.Sum(n => n.TotalIngresos) : 0,
                TotalDeducciones = nominasDelMesActual.Any() ? (decimal)nominasDelMesActual.Sum(n => n.TotalDeducciones) : 0,
                TotalImpuestos = nominasDelMesActual.Any() ? (decimal)nominasDelMesActual.Sum(n => n.TotalImpuestos) : 0,
                TotalEmpleadosEnNomina = (int)(nominasDelMesActual.Any() ? nominasDelMesActual.Sum(n => n.TotalEmpleadosEnNomina) : 0)
            };

            return totales;
        }
        private async Task<List<VacacionDetalle>> SolicitudTomarAccion()
        {
            var empleadoAusencias = await _context.VacacionDetalles
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdVacacionNavigation)
                .Where(e => e.Activo && e.EstadoSolicitud == Estado.Pendiente)
                .ToListAsync();

            return empleadoAusencias;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEstadoSolicitud([FromBody] VacacionDetalle request)
        {
            var vacacionDetalle = await _context.VacacionDetalles.FindAsync(request.IdVacacionDetalle);
            if (vacacionDetalle == null)
            {
                return Json(new { success = false, message = "Solicitud de vacaciones no encontrada." });
            }

            // Acceder al usuario actual
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado." });
            }

            // Actualizar el estado, los días aprobados y los comentarios
            vacacionDetalle.EstadoSolicitud = request.EstadoSolicitud;
            vacacionDetalle.DiasAprobados = request.DiasAprobados;
            vacacionDetalle.ComentariosAprobador = request.ComentariosAprobador;
            vacacionDetalle.AprobadoPor = usuarioActual.Email; // Asignar directamente el usuario actual

            // Establecer campos de auditoría
            SetCamposAuditoria2(vacacionDetalle, false); // false indica que no es un nuevo registro

            _context.Update(vacacionDetalle);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Estado actualizado correctamente." });
        }
        private void SetCamposAuditoria2(VacacionDetalle record, bool isNewRecord)
        {
            var now = DateTime.Now;
            var currentUser = _userManager.GetUserName(User);

            if (isNewRecord)
            {
                record.FechaCreacion = now;
                record.CreadoPor = currentUser;
            }

            record.FechaModificacion = now;
            record.ModificadoPor = currentUser;
        }

        private async Task<List<Nomina>> NominaTomarAccion()
        {
            var empleadoNominas = await _context.Nominas             
                .Include(e => e.IdTipoNominaNavigation)
                .Where(e => e.Activo && e.EstadoNomina == Nomina.NominaEstado.AprobacionPendiente)
                .ToListAsync();

            return empleadoNominas;
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarEstadoNomina([FromBody] Nomina request)
        {
            var nomina = await _context.Nominas.FindAsync(request.IdNomina);

            if (nomina == null)
            {
                return Json(new { success = false, message = "Nómina no encontrada." });
            }
            // Acceder al usuario actual
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado." });
            }

            nomina.EstadoNomina = request.EstadoNomina;
            nomina.AprobadaPor = usuarioActual.Email;
            nomina.ComentariosAprobador = request.ComentariosAprobador;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }


        private async Task<List<Viatico>> SolicitudViaticoTomarAccion()
        {
            var empleadoViaticos = await _context.Viaticos
                .Include(e => e.IdEmpleadoNavigation)              
                .Where(e => e.Activo && e.Estado == TipoEstado.Pendiente)
                .ToListAsync();

            return empleadoViaticos;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarEstadoViatico([FromBody] NominaTotales request)
        {
            var empleadoViaticos = await _context.Viaticos.FindAsync(request.Id);
            if (empleadoViaticos == null)
            {
                return Json(new { success = false, message = "Viatico no encontrad." });
            }

            // Acceder al usuario actual
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado." });
            }

            // Actualizar el estado y el usuario que aprueba la ausencia
            empleadoViaticos.Estado = request.EstadoViatico;
            empleadoViaticos.AprobadoPor = usuarioActual.Email; // Asignar directamente el usuario actual
            empleadoViaticos.FechaAprobacion = DateOnly.FromDateTime(DateTime.Now); 


            // Establecer campos de auditoría
            SetCamposAuditoria3(empleadoViaticos, false); 

            _context.Update(empleadoViaticos);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Estado actualizado correctamente." });
        }
        private void SetCamposAuditoria3(Viatico record, bool bNewRecord)
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


    //index
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
                    .ToListAsync();
                    return proximosCumpleañeros;
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                    return new List<Empleado>();
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

        private async Task<List<InicioFiltros>> EmpleadoDepartamento()
        {
            var departamentoCount = await _context.Empleados
                .Include(e => e.IdDepartamentoNavigation)
                .Where(e => e.Activo)
                .GroupBy(e => e.IdDepartamentoNavigation.NombreDepartamento)
                .Select(g => new InicioFiltros
                {
                    NombreDepartamento = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return departamentoCount;
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


        [HttpGet]
        public async Task<IActionResult> ComprobarPeriodoVacacional(int id, int diasAprobados, int idvacacion)
        {
            var vacacionDetalles = await _context.VacacionDetalles
                .Include(v => v.IdVacacionNavigation)
                .FirstOrDefaultAsync(v => v.IdVacacionDetalle == id);

            if (vacacionDetalles == null)
            {
                return Json(new { success = false, message = "Vacación no encontrada." });
            }

            var periodoVacacional = vacacionDetalles.IdVacacionNavigation.PeriodoVacacional;

            var vacacion = await _context.Vacacions
               .Include(v => v.IdEmpleadoNavigation)
               .FirstOrDefaultAsync(v => v.IdVacacion == idvacacion);

            if (vacacion == null)
            {
                return Json(new { success = false, message = "Vacación no encontrada." });
            }

            if (periodoVacacional == vacacion.PeriodoVacacional)
            {
                // Actualizar el valor de DiasGozados
                vacacion.DiasGozados += diasAprobados;
                vacacion.DiasPendientes -= diasAprobados;

                try
                {
                    _context.Update(vacacion);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Periodo vacacional comprobado y días gozados actualizados correctamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al actualizar los días gozados: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "El periodo vacacional no coincide con el año actual." });
            }
        }


        public IActionResult NoPermissionAccess()
        {
            return View();
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

