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

namespace PlanillaPM.Controllers
{
    public class VacacionController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public VacacionController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, int? estado)
        {
            IQueryable<Vacacion> query = _context.Vacacions;

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

            return View(data);

        }


        public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<Vacacion>? data = null;
             if (data == null)
             {
                data = _context.Vacacions.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "Vacacions.xlsx";
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
        // GET: Vacacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacion = await _context.Vacacions
                .Include(v => v.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdVacacion == id);
            if (vacacion == null)
            {
                return NotFound();
            }

            return View(vacacion);
        }

        // GET: Vacacion/Create
        public IActionResult Create()
        {
            
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            return View();
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVacacion,IdEmpleado,Observaciones,PeriodoVacacional,TotalDiasPeriodo,DiasGozados,DiasPendientes,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Vacacion vacacion)
        {
            if (ModelState.IsValid)
            {
                // Obtener el empleado
                var empleado = await _context.Empleados.FindAsync(vacacion.IdEmpleado);
                if (empleado == null)
                {
                    TempData["error"] = "Error: Empleado no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar antigüedad del empleado
                int antiguedad = empleado.Antiguedad;

                // Obtener el año del PeriodoVacacional
                int periodoVacacionalYear = vacacion.PeriodoVacacional;

                // Comparar con la fecha de inicio del empleado si no es nulo
                if (antiguedad == 0 || (empleado.FechaInicio.HasValue && empleado.FechaInicio.Value.Year == periodoVacacionalYear))
                {
                    TempData["error"] = "Error: El empleado aún no cumple los requisitos de antigüedad para tener días de vacaciones.";
                    ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacion.IdEmpleado);
                    return View(vacacion);
                }

                SetCamposAuditoria(vacacion, true);
                _context.Add(vacacion);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacion.IdEmpleado);
            return View(vacacion);
        }

        [HttpGet]
        public async Task<JsonResult> GetAntiguedad(int idEmpleado)
        {
            var empleado = await _context.Empleados.FindAsync(idEmpleado);
            if (empleado == null)
            {
                return Json(new { antiguedad = 0 });
            }

            return Json(new { antiguedad = empleado.Antiguedad });
        }


        [HttpGet]
        public async Task<JsonResult> CalcularVacaciones(int idEmpleado, int year)
        {
            try
            {
                var empleado = await _context.Empleados.FindAsync(idEmpleado);
                if (empleado == null)
                {
                    return Json(new { success = false, message = "Empleado no encontrado." });
                }

                int antiguedad = empleado.Antiguedad;

                if (antiguedad == 0)
                {
                    return Json(new { success = false, message = "El empleado aún no cumple los requisitos de antigüedad para tener días de vacaciones." });
                }

                // Obtener los días de vacaciones según la antigüedad
                var diasVacacion = await _context.DiasVacaciones
                    .Where(dv => dv.Hasta >= antiguedad && dv.Activo)
                    .OrderBy(dv => dv.Hasta)
                    .FirstOrDefaultAsync();

                if (diasVacacion == null)
                {
                    return Json(new { success = false, message = "No se encontraron días de vacaciones para la antigüedad del empleado." });
                }

                // Obtener los detalles de vacaciones del empleado
                var detallesVacaciones = await _context.Vacacions
                    .Where(v => v.IdEmpleado == idEmpleado && v.Activo)
                    .ToListAsync();

                int diasGozados = detallesVacaciones.Where(v => v.PeriodoVacacional == year).Sum(v => v.DiasGozados);
                int diasPendientes = diasVacacion.DiasVacaciones - diasGozados;

                // Verificar días pendientes del año actual o anterior
                var historialVacaciones = detallesVacaciones
                    .Where(v => v.PeriodoVacacional == year || (v.DiasPendientes > 0 && v.PeriodoVacacional < year))
                    .ToList();

                // Sumar días pendientes de años anteriores si existen
                int diasPendientesAnteriores = historialVacaciones
                    .Where(v => v.DiasPendientes > 0 && v.PeriodoVacacional < year)
                    .Sum(v => v.DiasPendientes);

                diasPendientes += diasPendientesAnteriores;

                return Json(new
                {
                    success = true,
                    antiguedad,
                    diasVacaciones = diasVacacion.DiasVacaciones,
                    diasGozados,
                    diasPendientes,
                    historial = historialVacaciones.Select(v => new
                    {
                        v.IdVacacion,
                        v.PeriodoVacacional,
                        v.TotalDiasPeriodo,
                        v.DiasPendientes,
                        v.DiasGozados
                    })
                });
            }
            catch (Exception ex)
            {
                // Mostrar error en consola
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "Ocurrió un error al calcular las vacaciones.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> BuscarHistorialVacaciones(int idEmpleado)
        {
            try
            {
                var historialVacaciones = await _context.Vacacions
                    .Where(vd => vd.IdEmpleado == idEmpleado && vd.Activo)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    historial = historialVacaciones.Select(vd => new
                    {
                        IdVacacion = vd.IdVacacion,
                        PeriodoVacacional = vd.PeriodoVacacional,
                        TotalDiasPeriodo = vd.TotalDiasPeriodo,
                        DiasPendientes = vd.DiasPendientes,
                        DiasGozados = vd.DiasGozados
                    })
                });
            }
            catch (Exception ex)
            {
                // Mostrar error en consola
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "Ocurrió un error al buscar el historial de vacaciones.", error = ex.Message });
            }
        }


        // GET: Vacacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacion = await _context.Vacacions.FindAsync(id);
            if (vacacion == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacion.IdEmpleado);
            return View(vacacion);
        }

        // POST: Vacacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVacacion,IdEmpleado,Observaciones,PeriodoVacacional,TotalDiasPeriodo,DiasGozados,DiasPendientes,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Vacacion vacacion)
        {
            if (id != vacacion.IdVacacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(vacacion, false);
                    _context.Update(vacacion);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacacionExists(vacacion.IdVacacion))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacion.IdEmpleado);
            return View(vacacion);
        }

        // GET: Vacacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacion = await _context.Vacacions
                .Include(v => v.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdVacacion == id);
            if (vacacion == null)
            {
                return NotFound();
            }

            return View(vacacion);
        }

        // POST: Vacacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var vacacion = await _context.Vacacions.FindAsync(id);
            try
            {
               
                if (vacacion != null)
                {
                    _context.Vacacions.Remove(vacacion);
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
                return View(vacacion);
            }

        }

        private bool VacacionExists(int id)
        {
            return _context.Vacacions.Any(e => e.IdVacacion == id);
        }
        
        private void SetCamposAuditoria(Vacacion record, bool bNewRecord)
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
