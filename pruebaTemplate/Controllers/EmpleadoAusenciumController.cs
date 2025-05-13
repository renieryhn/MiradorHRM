using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PlanillaPM.Models;
using ClosedXML.Excel;
using static PlanillaPM.cGeneralFun;
using System.Data;
using static PlanillaPM.Models.EmpleadoAusencium;

namespace PlanillaPM.Controllers
{
    public class EmpleadoAusenciumController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoAusenciumController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoAusencium
        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, int? estado)
        {
            IQueryable<EmpleadoAusencium> query = _context.EmpleadoAusencia;
  
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
                // No hace falta ningún filtro si el estado es null o no es 0 ni 1 (es decir, se quieren mostrar todos los registros)
            }
            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdTipoAusenciaNavigation.NombreTipoAusencia.ToLower().Contains(filter.ToLower()));
            }

            ViewBag.Filter = filter;
            ViewBag.CurrentIdEmpleado = idEmpleado;
            ViewBag.CurrentEstado = estado;

            List<EmpleadoAusencium> registros;
            registros = await query.Include(e => e.IdEmpleadoNavigation).ToListAsync();


            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
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

            var IdTipoAusenciaNavigation = await _context.TipoAusencia.ToListAsync();
            //var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            return View(data);

        }

        [HttpGet]
        public ActionResult Download(int id)
        {
            // Filtrar las ausencias del empleado por el IdEmpleado recibido
            var data = _context.EmpleadoAusencia
                .Where(ec => ec.IdEmpleado == id)
                .Select(ec => new
                {
                    ec.IdEmpleadoAusencia,
                    Empleado = ec.IdEmpleadoNavigation.NombreEmpleado + " " + ec.IdEmpleadoNavigation.ApellidoEmpleado,
                    TipoAusencia = ec.IdTipoAusenciaNavigation.NombreTipoAusencia,
                    ec.DiaCompleto,
                    EstadoAusencia = ec.Estado.ToString(),
                    FechaDesde = ec.FechaDesde.ToString("yyyy-MM-dd"),
                    FechaHasta = ec.FechaHasta.ToString("yyyy-MM-dd"),
                    HoraDesde = ec.HoraDesde.HasValue ? ec.HoraDesde.Value.ToString("HH:mm") : "N/A",
                    HoraHasta = ec.HoraHasta.HasValue ? ec.HoraHasta.Value.ToString("HH:mm") : "N/A",
                    ec.AprobadoPor,
                    ec.Comentarios,
                    Activo = ec.Activo ? "Sí" : "No"
                    
                })
                .ToList();

          

            // Convertir la lista de ausencias en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = $"EmpleadoAusencia_{id}.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table, "EmpleadoAusencia");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    // Devolver el archivo como una descarga de archivo Excel
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }


        [HttpGet]
        public ActionResult DownloadAll(string? filter, string? idEmpleado, int? estado)
        {
            IQueryable<EmpleadoAusencium> query = _context.EmpleadoAusencia
                .Include(ec => ec.IdEmpleadoNavigation)
                .Include(ec => ec.IdTipoAusenciaNavigation);

            if (!string.IsNullOrEmpty(idEmpleado))
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

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdTipoAusenciaNavigation.NombreTipoAusencia.ToLower().Contains(filter.ToLower()));
            }

            var data = query
                .Select(ec => new
                {
                    ec.IdEmpleadoAusencia,
                    Empleado = ec.IdEmpleadoNavigation.NombreEmpleado + " " + ec.IdEmpleadoNavigation.ApellidoEmpleado,
                    TipoAusencia = ec.IdTipoAusenciaNavigation.NombreTipoAusencia,
                    ec.DiaCompleto,
                    EstadoAusencia = ec.Estado.ToString(),
                    FechaDesde = ec.FechaDesde.ToString("yyyy-MM-dd"),
                    FechaHasta = ec.FechaHasta.ToString("yyyy-MM-dd"),
                    HoraDesde = ec.HoraDesde.HasValue ? ec.HoraDesde.Value.ToString("HH:mm") : "N/A",
                    HoraHasta = ec.HoraHasta.HasValue ? ec.HoraHasta.Value.ToString("HH:mm") : "N/A",
                    ec.AprobadoPor,
                    ec.Comentarios,
                    Activo = ec.Activo ? "Sí" : "No"
                })
                .ToList();

            if (!data.Any())
            {
                TempData["error"] = "No se encontraron registros con los filtros aplicados.";
                return RedirectToAction(nameof(Index), new { filter, idEmpleado, estado });
            }

            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);
            string fileName = "EmpleadoAusencia.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table, "EmpleadoAusencia");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                fileName);
                }
            }
        }


        // GET: EmpleadoAusencium/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoAusencium = await _context.EmpleadoAusencia
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdTipoAusenciaNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoAusencia == id);
            if (empleadoAusencium == null)
            {
                return NotFound();
            }

            return View(empleadoAusencium);
        }

        // GET: EmpleadoAusencium/Create
        public IActionResult Create()
        {

            //ViewBag.EstadoAusencia = Enum.GetValues(typeof(EstadoAusencia));
            ViewBag.EstadoAusencia = new[] { EstadoAusencia.Solicitada };
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            ViewData["IdTipoAusencia"] = new SelectList(_context.TipoAusencia, "IdTipoAusencia", "NombreTipoAusencia");
            return View();
        }

        // POST: EmpleadoAusencium/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoAusencia,IdEmpleado,IdTipoAusencia,DiaCompleto,Estado,FechaDesde,FechaHasta,HoraDesde,HoraHasta,AprobadoPor,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoAusencium empleadoAusencium, int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoAusencium, true);
                    _context.Add(empleadoAusencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se creó exitosamente.";

                   
                    if (id.HasValue)
                    {

                        if (id == 1)
                        {
                            return Redirect($"/Empleado/FichaEmpleado/{empleadoAusencium.IdEmpleado}?tab=Ausencias");
                        }
                        if (id == 2)
                        {
                            return RedirectToAction("Index");
                        }

                    }
                    else
                    {
                        TempData["error"] = "Error no se encontro el valor de la direccion";
                        return RedirectToAction("Index");
                    }
                }
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoAusencium.IdEmpleado);
                ViewData["IdTipoAusencia"] = new SelectList(_context.TipoAusencia, "IdTipoAusencia", "NombreTipoAusencia", empleadoAusencium.IdTipoAusencia);

                // Si el modelo no es válido pero no se ha lanzado una excepción,
                // agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, revise los campos.";

                return View(empleadoAusencium);
            }
            catch (Exception ex)
            {
                // En caso de una excepción, agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un error inesperado al intentar crear el registro. Por favor, intente nuevamente.";

                // Log.Error($"Error al crear el registro: {ex.Message}", ex);

                return View();
            }
        }

        // GET: EmpleadoAusencium/Edit/5
        public async Task<IActionResult> Edit(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoAusencium = await _context.EmpleadoAusencia.FindAsync(id);
            if (empleadoAusencium == null)
            {
                return NotFound();
            }

            ViewBag.Numero = numero;
            ViewBag.EstadoAusencia = Enum.GetValues(typeof(EmpleadoAusencium.EstadoAusencia));
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoAusencium.IdEmpleado);
            ViewData["IdTipoAusencia"] = new SelectList(_context.TipoAusencia, "IdTipoAusencia", "NombreTipoAusencia", empleadoAusencium.IdTipoAusencia);
            return View(empleadoAusencium);
        }

        // POST: EmpleadoAusencium/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoAusencia,IdEmpleado,IdTipoAusencia,DiaCompleto,Estado,FechaDesde,FechaHasta,HoraDesde,HoraHasta,AprobadoPor,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoAusencium empleadoAusencium, string? numero)
        {
            try
            {
                if (id != empleadoAusencium.IdEmpleadoAusencia)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoAusencium, false);
                    _context.Update(empleadoAusencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se actualizó exitosamente.";
                 
                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoAusencium.IdEmpleado}?tab=Ausencias");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
                }

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoAusencium.IdEmpleado);
                ViewData["IdTipoAusencia"] = new SelectList(_context.TipoAusencia, "IdTipoAusencia", "NombreTipoAusencia", empleadoAusencium.IdTipoAusencia);

                // Si el modelo no es válido pero no se ha lanzado una excepción,
                // agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar editar el registro. Por favor, revise los campos.";

                return View(empleadoAusencium);
            }
            catch (Exception ex)
            {
                // En caso de una excepción, agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un error inesperado al intentar editar el registro. Por favor, intente nuevamente.";

                // Log.Error($"Error al editar el registro: {ex.Message}", ex);

                return View();
            }
        }

        // GET: EmpleadoAusencium/Delete/5
        public async Task<IActionResult> Delete(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoAusencium = await _context.EmpleadoAusencia
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdTipoAusenciaNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoAusencia == id);
            if (empleadoAusencium == null)
            {
                return NotFound();
            }

            ViewBag.Numero = numero;

            return View(empleadoAusencium);
        }

        // POST: EmpleadoAusencium/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? numero)
        {
            try
            {
                var empleadoAusencium = await _context.EmpleadoAusencia.FindAsync(id);
                if (empleadoAusencium != null)
                {
                    _context.EmpleadoAusencia.Remove(empleadoAusencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se eliminó exitosamente.";
                }

                if (numero == "1")
                {
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoAusencium.IdEmpleado}?tab=Ausencias");
                }
                if (numero == "2")
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // En caso de una excepción, agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un error al intentar eliminar el registro. Por favor, intente nuevamente.";

                // Log.Error($"Error al eliminar el registro: {ex.Message}", ex);

                return View();
            }
            return View();
        }

        private bool EmpleadoAusenciumExists(int id)
        {
            return _context.EmpleadoAusencia.Any(e => e.IdEmpleadoAusencia == id);
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
    }
}
