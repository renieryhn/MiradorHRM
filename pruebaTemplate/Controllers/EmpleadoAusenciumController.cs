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
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            //var planillaContext = _context.EmpleadoAusencia.Include(e => e.IdEmpleadoNavigation).Include(e => e.IdTipoAusenciaNavigation);
            //return View(await planillaContext.ToListAsync());

            List<EmpleadoAusencium> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoAusencia.Where(r => r.AprobadoPor.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoAusencia.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);

        }

        public ActionResult Download()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            List<EmpleadoAusencium>? data = null;
            if (data == null)
            {
                data = _context.EmpleadoAusencia.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "EmpleadoAusencia.xlsx";
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado");
            ViewData["IdTipoAusencia"] = new SelectList(_context.TipoAusencia, "IdTipoAusencia", "NombreTipoAusencia");
            return View();
        }

        // POST: EmpleadoAusencium/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoAusencia,IdEmpleado,IdTipoAusencia,DiaCompleto,Estado,FechaDesde,FechaHasta,HoraDesde,HoraHasta,AprobadoPor,Comentarios,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoAusencium empleadoAusencium)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoAusencium, true);
                    _context.Add(empleadoAusencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["mensaje"] = "El registro se creó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoAusencium.IdEmpleado);
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

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: EmpleadoAusencium/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoAusencium.IdEmpleado);
            ViewData["IdTipoAusencia"] = new SelectList(_context.TipoAusencia, "IdTipoAusencia", "NombreTipoAusencia", empleadoAusencium.IdTipoAusencia);
            return View(empleadoAusencium);
        }

        // POST: EmpleadoAusencium/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoAusencia,IdEmpleado,IdTipoAusencia,DiaCompleto,Estado,FechaDesde,FechaHasta,HoraDesde,HoraHasta,AprobadoPor,Comentarios,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoAusencium empleadoAusencium)
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
                    TempData["mensaje"] = "El registro se actualizó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoAusencium.IdEmpleado);
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

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: EmpleadoAusencium/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: EmpleadoAusencium/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var empleadoAusencium = await _context.EmpleadoAusencia.FindAsync(id);
                if (empleadoAusencium != null)
                {
                    _context.EmpleadoAusencia.Remove(empleadoAusencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["mensaje"] = "El registro se eliminó exitosamente.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // En caso de una excepción, agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un error al intentar eliminar el registro. Por favor, intente nuevamente.";

                // Log.Error($"Error al eliminar el registro: {ex.Message}", ex);

                return RedirectToAction(nameof(Index));
            }
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
