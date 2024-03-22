using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using static PlanillaPM.cGeneralFun;

namespace PlanillaPM.Controllers
{
    public class EmpleadoHabilidadController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoHabilidadController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoHabilidad
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            //var planillaContext = _context.EmpleadoHabilidads.Include(e => e.IdEmpleadoNavigation);
            //return View(await planillaContext.ToListAsync());

            List<EmpleadoHabilidad> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoHabilidads.Where(r => r.Habilidad.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoHabilidads.ToListAsync();
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
            List<EmpleadoHabilidad>? data = null;
            if (data == null)
            {
                data = _context.EmpleadoHabilidads.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "EmpleadoHabilidad.xlsx";
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

        // GET: EmpleadoHabilidad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHabilidad = await _context.EmpleadoHabilidads
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoHabilidad == id);
            if (empleadoHabilidad == null)
            {
                return NotFound();
            }

            return View(empleadoHabilidad);
        }

        // GET: EmpleadoHabilidad/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado");
            return View();
        }

        // POST: EmpleadoHabilidad/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoHabilidad,IdEmpleado,Habilidad,ExperienciaYears,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoHabilidad empleadoHabilidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoHabilidad, true);
                    _context.Add(empleadoHabilidad);
                    await _context.SaveChangesAsync();

                    // Mensaje de éxito
                    TempData["mensaje"] = "La habilidad del empleado se creó exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {

                // Mensaje de error
                TempData["Error"] = "Ha ocurrido un error al intentar guardar la información. Por favor, inténtelo de nuevo.";
            }

            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoHabilidad.IdEmpleado);
            return View(empleadoHabilidad);
        }

        // GET: EmpleadoHabilidad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHabilidad = await _context.EmpleadoHabilidads.FindAsync(id);
            if (empleadoHabilidad == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoHabilidad.IdEmpleado);
            return View(empleadoHabilidad);
        }

        // POST: EmpleadoHabilidad/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoHabilidad,IdEmpleado,Habilidad,ExperienciaYears,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoHabilidad empleadoHabilidad)
        {
            try
            {
                if (id != empleadoHabilidad.IdEmpleadoHabilidad)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoHabilidad, false);
                    _context.Update(empleadoHabilidad);
                    await _context.SaveChangesAsync();

                    // Mensaje de éxito
                    TempData["mensaje"] = "La habilidad del empleado se ha actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoHabilidadExists(empleadoHabilidad.IdEmpleadoHabilidad))
                {
                    return NotFound();
                }
                else
                {

                    // Mensaje de error
                    TempData["Error"] = "Ha ocurrido un error al intentar actualizar la información. Por favor, inténtelo de nuevo.";
                }
            }

            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoHabilidad.IdEmpleado);
            return View(empleadoHabilidad);
        }

        // GET: EmpleadoHabilidad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHabilidad = await _context.EmpleadoHabilidads
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoHabilidad == id);
            if (empleadoHabilidad == null)
            {
                return NotFound();
            }

            return View(empleadoHabilidad);
        }

        // POST: EmpleadoHabilidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var empleadoHabilidad = await _context.EmpleadoHabilidads.FindAsync(id);
                if (empleadoHabilidad != null)
                {
                    _context.EmpleadoHabilidads.Remove(empleadoHabilidad);
                    await _context.SaveChangesAsync();

                    // Mensaje de éxito
                    TempData["mensaje"] = "La habilidad del empleado se ha eliminado exitosamente.";
                }
            }
            catch (Exception ex)
            {

                // Mensaje de error
                TempData["Error"] = "Ha ocurrido un error al intentar eliminar la habilidad del empleado. Por favor, inténtelo de nuevo.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoHabilidadExists(int id)
        {
            return _context.EmpleadoHabilidads.Any(e => e.IdEmpleadoHabilidad == id);
        }

        private void SetCamposAuditoria(EmpleadoHabilidad record, bool bNewRecord)
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
