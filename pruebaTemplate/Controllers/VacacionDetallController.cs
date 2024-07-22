using System.Data;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static PlanillaPM.cGeneralFun;
using Microsoft.AspNetCore.Identity;
using PlanillaPM.ViewModel;

using PlanillaPM.Models;
using static PlanillaPM.Models.VacacionDetalle;
using System.Runtime.InteropServices.Marshalling;

namespace PlanillaPM.Controllers
{
    public class VacacionDetallController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public VacacionDetallController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: VacacionDetall

        [HttpGet]
        public IActionResult Getvacaciondetalles()
        {
            // Obtener todos los días festivos de la base de datos
            var vacaciondetalles = _context.VacacionDetalles
             .Include(v => v.IdEmpleadoNavigation) // Incluye la navegación a Empleado
             .ToList();

            // Devolver los días festivos en formato JSON
            return Json(vacaciondetalles);
        }
        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, int? estado)
        {
            IQueryable<VacacionDetalle> query = _context.VacacionDetalles;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdEmpleadoNavigation.NombreCompleto.ToLower().Contains(filter.ToLower()));
            }
            if (!String.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdEmpleado.ToString().Contains(idEmpleado));
            }


            if (estado.HasValue)
            {
                if (estado == 1)
                {
                    query = query.Where(r => r.Activo == false);
                }
                else if (estado == 0)
                {
                    query = query.Where(r => r.Activo == true);
                }

            }

            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentIdEmpleado = idEmpleado;
            ViewBag.CurrentEstado = estado;


            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = query.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = query.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            if (idEmpleado != null)
            {
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            }
            else
            {
                ViewData["IdEmpleado"] = new SelectList(IdEmpleadoNavigation, "IdEmpleado", "NombreCompleto");
            }
            var planillaContext = await _context.EmpleadoDeduccions.ToListAsync();
            var IdVacacionNavigation = await _context.Vacacions.ToListAsync();

            return View(data);

        }
        public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<VacacionDetalle>? data = null;
             if (data == null)
             {
                data = _context.VacacionDetalles.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "VacacionDetalles.xlsx";
             using (XLWorkbook wb = new XLWorkbook())
             {
                 wb.Worksheets.Add(table);
                 using (MemoryStream stream = new MemoryStream())
                 {
                     wb.SaveAs(stream);
                     return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                 }
             }
        }    
        // GET: VacacionDetall/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacionDetalle = await _context.VacacionDetalles
                .Include(v => v.IdEmpleadoNavigation)
                .Include(v => v.IdVacacionNavigation)
                .FirstOrDefaultAsync(m => m.IdVacacionDetalle == id);
            if (vacacionDetalle == null)
            {
                return NotFound();
            }

            return View(vacacionDetalle);
        }

        // GET: VacacionDetall/Create
        public IActionResult Create()
        {
            ViewBag.Estado = Enum.GetValues(typeof(Estado));
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            ViewData["IdVacacion"] = new SelectList(Enumerable.Empty<SelectListItem>(), "IdVacacion", "PeriodoVacacional");

            return View();
        }
        [HttpGet]
        public JsonResult GetVacacionesByEmpleado(int idEmpleado)
        {
            var vacaciones = _context.Vacacions
                .Where(v => v.IdEmpleado == idEmpleado)
                .Select(v => new { v.IdVacacion, v.PeriodoVacacional })
                .ToList();

            return Json(vacaciones);
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerDiasDisponibles(int idEmpleado, int idVacacion)
        {
            // Obtiene el período vacacional y los días totales permitidos
            var periodoVacacional = await _context.Vacacions
                .Where(v => v.IdVacacion == idVacacion)
                .FirstOrDefaultAsync();

            if (periodoVacacional == null)
            {
                return Json(new { success = false, message = "Período vacacional no encontrado." });
            }

            // Obtiene todas las vacaciones solicitadas por el empleado en ese período
            var vacacionesSolicitadas = await _context.Vacacions
                .Where(v => v.IdEmpleado == idEmpleado && v.IdVacacion == idVacacion)
                .ToListAsync();

            var diasfestivos = await _context.DiaFestivos.ToListAsync();

            

            // Suma los días solicitados
            int diasUsados = vacacionesSolicitadas.Sum(v => v.DiasPendientes);

            // Calcula los días disponibles
            int diasDisponibles = diasUsados - periodoVacacional.DiasTotales;

            //int porcentaje = (diasUsados/ periodoVacacional.TotalDiasPeriodo)*100;
            int porcentaje = (diasUsados * 100) / periodoVacacional.TotalDiasPeriodo;

            return Json(new { success = true, diasDisponibles, porcentaje });
        }

        public async Task<int> CalcularDiasFestivosAsync(DateOnly fechaInicio, DateOnly fechaFin)
        {
            var diasFestivos = await _context.DiaFestivos
                .Where(df => df.FechaDesde <= fechaFin && df.FechaHasta >= fechaInicio)
                .ToListAsync();

            int diasFestivosEnRango = diasFestivos
                .Where(df => (df.FechaDesde >= fechaInicio && df.FechaDesde <= fechaFin) ||
                             (df.FechaHasta >= fechaInicio && df.FechaHasta <= fechaFin))
                .Sum(df => (df.FechaHasta.ToDateTime(new TimeOnly(0, 0)) - df.FechaDesde.ToDateTime(new TimeOnly(0, 0))).Days + 1);

            return diasFestivosEnRango;
        }


        [HttpPost]
        public async Task<IActionResult> CalcularDiasVacaciones([FromBody] CalcularDiasVacacionesRequest request)
        {
            try
            {
                DateOnly fechaInicio = DateOnly.Parse(request.FechaInicio);
                DateOnly fechaFin = DateOnly.Parse(request.FechaFin);

                var empleado = await _context.Empleados
                    .Include(e => e.IdClaseEmpleadoNavigation)
                    .ThenInclude(ce => ce.IdHorarioNavigation)
                    .FirstOrDefaultAsync(e => e.IdEmpleado == request.IdEmpleado);

                if (empleado == null || empleado.IdClaseEmpleadoNavigation == null || empleado.IdClaseEmpleadoNavigation.IdHorarioNavigation == null)
                {
                    return Json(new { success = false, message = "No se encontró el empleado o no tiene una clase asignada con un horario." });
                }

                var horario = empleado.IdClaseEmpleadoNavigation.IdHorarioNavigation;
                var diasFestivos = await CalcularDiasFestivosAsync(fechaInicio, fechaFin);

                var diasLaborables = new List<DateOnly>();
                var diasNoLaborables = new List<DateOnly>();

                for (var date = fechaInicio; date <= fechaFin; date = date.AddDays(1))
                {
                    bool esLaborable = false;
                    switch (date.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            esLaborable = horario.IndLunes;
                            break;
                        case DayOfWeek.Tuesday:
                            esLaborable = horario.IndMartes;
                            break;
                        case DayOfWeek.Wednesday:
                            esLaborable = horario.IndMiercoles;
                            break;
                        case DayOfWeek.Thursday:
                            esLaborable = horario.IndJueves;
                            break;
                        case DayOfWeek.Friday:
                            esLaborable = horario.IndViernes;
                            break;
                        case DayOfWeek.Saturday:
                            esLaborable = horario.IndSabado;
                            break;
                        case DayOfWeek.Sunday:
                            esLaborable = horario.IndDomingo;
                            break;
                    }

                    if (esLaborable)
                    {
                        diasLaborables.Add(date);
                    }
                    else
                    {
                        diasNoLaborables.Add(date);
                    }
                }

                int totalDiasNoLaborables = diasNoLaborables.Count;
                int diasAprobados = request.NumeroDiasSolicitados - diasFestivos - totalDiasNoLaborables;
                if (diasAprobados < 0) diasAprobados = 0;

                return Json(new
                {
                    success = true,
                    diasAprobados,
                    diasFestivos,
                    diasLaborables,
                    diasNoLaborables
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "Ocurrió un error al calcular los días de vacaciones.", error = ex.Message });
            }
        }

        public class CalcularDiasVacacionesRequest
        {
            public int IdEmpleado { get; set; }
            public string FechaInicio { get; set; }
            public string FechaFin { get; set; }
            public int NumeroDiasSolicitados { get; set; }
        }
 


        // POST: VacacionDetall/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVacacionDetalle,IdVacacion,IdEmpleado,FechaSolicitud,FechaInicio,FechaFin,NumeroDiasSolicitados,EstadoSolicitud,AprobadoPor,DiasAprobados,ComentariosAprobador,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] VacacionDetalle vacacionDetalle)
        {
            if (ModelState.IsValid)
            {

            
                var vacacion = _context.Vacacions
                    .FirstOrDefault(v => v.IdEmpleado == vacacionDetalle.IdEmpleado && v.IdVacacion == vacacionDetalle.IdVacacion);

                // Si hay menos días pendientes que los solicitados, mostrar error
                if (vacacion != null && vacacion.DiasPendientes < vacacionDetalle.NumeroDiasSolicitados || vacacionDetalle.NumeroDiasSolicitados == 0)
                {
                    TempData["error"] = "Error: El empleado no cuenta con suficientes días o el numero de dias es cero(0).";
                    return RedirectToAction(nameof(Index));
                }

                bool tieneSolicitudPendiente = _context.VacacionDetalles
                .Any(v => v.IdEmpleado == vacacionDetalle.IdEmpleado && v.EstadoSolicitud == Estado.Pendiente);

                if (tieneSolicitudPendiente)
                {
                    TempData["error"] = "Error: El empleado ya tiene una solicitud pendiente y no puede realizar otra.";
                } 
                else
                {
                    // Verificar si el rango de fechas ya ha sido solicitado
                    bool fechasSolapadas = _context.VacacionDetalles
                        .Any(v => v.IdEmpleado == vacacionDetalle.IdEmpleado &&
                                  ((vacacionDetalle.FechaInicio >= v.FechaInicio && vacacionDetalle.FechaInicio <= v.FechaFin) ||
                                   (vacacionDetalle.FechaFin >= v.FechaInicio && vacacionDetalle.FechaFin <= v.FechaFin) ||
                                   (vacacionDetalle.FechaInicio <= v.FechaInicio && vacacionDetalle.FechaFin >= v.FechaFin)));

                    if (fechasSolapadas)
                    {
                        TempData["error"] = "Error: El rango de fechas ya ha sido solicitado.";
                        return RedirectToAction(nameof(Index));
                    }

                    SetCamposAuditoria(vacacionDetalle, true);
                    _context.Add(vacacionDetalle);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacionDetalle.IdEmpleado);
            ViewData["IdVacacion"] = new SelectList(_context.Vacacions, "IdVacacion", "PeriodoVacacional", vacacionDetalle.IdVacacion);
            return View(vacacionDetalle);
        }

        // GET: VacacionDetall/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacionDetalle = await _context.VacacionDetalles.FindAsync(id);
            if (vacacionDetalle == null)
            {
                return NotFound();
            }
            ViewBag.Estado = Enum.GetValues(typeof(VacacionDetalle.Estado));
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacionDetalle.IdEmpleado);
            ViewData["IdVacacion"] = new SelectList(_context.Vacacions, "IdVacacion", "PeriodoVacacional", vacacionDetalle.IdVacacion);
            return View(vacacionDetalle);
        }

        // POST: VacacionDetall/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVacacionDetalle,IdVacacion,IdEmpleado,FechaSolicitud,FechaInicio,FechaFin,NumeroDiasSolicitados,EstadoSolicitud,AprobadoPor,DiasAprobados,ComentariosAprobador,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] VacacionDetalle vacacionDetalle)
        {
            if (id != vacacionDetalle.IdVacacionDetalle)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(vacacionDetalle, false);
                    _context.Update(vacacionDetalle);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacacionDetalleExists(vacacionDetalle.IdVacacionDetalle))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }            
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacionDetalle.IdEmpleado);
            ViewData["IdVacacion"] = new SelectList(_context.Vacacions, "IdVacacion", "PeriodoVacacional", vacacionDetalle.IdVacacion);
            return View(vacacionDetalle);
        }

        // GET: VacacionDetall/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacionDetalle = await _context.VacacionDetalles
                .Include(v => v.IdEmpleadoNavigation)
                .Include(v => v.IdVacacionNavigation)
                .FirstOrDefaultAsync(m => m.IdVacacionDetalle == id);
            if (vacacionDetalle == null)
            {
                return NotFound();
            }

            return View(vacacionDetalle);
        }

        // POST: VacacionDetall/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var vacacionDetalle = await _context.VacacionDetalles.FindAsync(id);
            try
            {
               
                if (vacacionDetalle != null)
                {
                    _context.VacacionDetalles.Remove(vacacionDetalle);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido eliminado exitosamente.";
                    return RedirectToAction(nameof(Index));
                } 
                else
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_"))
                {
                    TempData["error"] = "Error: No puede elimiar el registro actual ya que se encuentra relacionado a otro Registro.";
                }
                else
                {
                    var message = ex.InnerException;
                    TempData["error"] = "Error: " + message;
                }
                return View(vacacionDetalle);
            }

        }

        private bool VacacionDetalleExists(int id)
        {
            return _context.VacacionDetalles.Any(e => e.IdVacacionDetalle == id);
        }
        
        private void SetCamposAuditoria(VacacionDetalle record, bool bNewRecord)
        {
            var now = DateTime.Now;
            var CurrentUser =  _userManager.GetUserName(User);
           
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
    }
}
