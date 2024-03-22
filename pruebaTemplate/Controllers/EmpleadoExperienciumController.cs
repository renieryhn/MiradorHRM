using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using Microsoft.AspNetCore.Identity;
using static PlanillaPM.cGeneralFun;

namespace PlanillaPM.Controllers
{
    public class EmpleadoExperienciumController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoExperienciumController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoExperiencium
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            //var planillaContext = _context.EmpleadoExperiencia.Include(e => e.IdEmpleadoNavigation);
            //return View(await planillaContext.ToListAsync());

            List<EmpleadoExperiencium> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoExperiencia.Where(r => r.Empresa.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoExperiencia.ToListAsync();
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
            List<EmpleadoExperiencium>? data = null;
            if (data == null)
            {
                data = _context.EmpleadoExperiencia.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "EmpleadoExperiencia.xlsx";
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

        // GET: EmpleadoExperiencium/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoExperiencium = await _context.EmpleadoExperiencia
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoExperiencia == id);
            if (empleadoExperiencium == null)
            {
                return NotFound();
            }

            return View(empleadoExperiencium);
        }

        // GET: EmpleadoExperiencium/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado");
            return View();
        }

        // POST: EmpleadoExperiencium/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoExperiencia,IdEmpleado,Empresa,Cargo,FechaDesde,FechaHasta,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoExperiencium empleadoExperiencium)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoExperiencium, true);
                    _context.Add(empleadoExperiencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["mensaje"] = "El registro se creó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoExperiencium.IdEmpleado);
                return View(empleadoExperiencium);
            }
            catch (Exception ex)
            {

                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, intente nuevamente.";

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoExperiencium.IdEmpleado);
                return View(empleadoExperiencium);
            }
        }

        // GET: EmpleadoExperiencium/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoExperiencium = await _context.EmpleadoExperiencia.FindAsync(id);
            if (empleadoExperiencium == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoExperiencium.IdEmpleado);
            return View(empleadoExperiencium);
        }

        // POST: EmpleadoExperiencium/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoExperiencia,IdEmpleado,Empresa,Cargo,FechaDesde,FechaHasta,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoExperiencium empleadoExperiencium)
        {
            try
            {
                if (id != empleadoExperiencium.IdEmpleadoExperiencia)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoExperiencium, false);
                    _context.Update(empleadoExperiencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["mensaje"] = "El registro se actualizó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoExperiencium.IdEmpleado);
                return View(empleadoExperiencium);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar editar el registro. Por favor, intente nuevamente.";

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoExperiencium.IdEmpleado);
                return View(empleadoExperiencium);
            }
        }

        // GET: EmpleadoExperiencium/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoExperiencium = await _context.EmpleadoExperiencia
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoExperiencia == id);
            if (empleadoExperiencium == null)
            {
                return NotFound();
            }

            return View(empleadoExperiencium);
        }

        // POST: EmpleadoExperiencium/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var empleadoExperiencium = await _context.EmpleadoExperiencia.FindAsync(id);
                if (empleadoExperiencium != null)
                {
                    _context.EmpleadoExperiencia.Remove(empleadoExperiencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["mensaje"] = "El registro se eliminó exitosamente.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                
                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar eliminar el registro. Por favor, intente nuevamente.";

                return RedirectToAction(nameof(Index));
            }
        }

        private bool EmpleadoExperienciumExists(int id)
        {
            return _context.EmpleadoExperiencia.Any(e => e.IdEmpleadoExperiencia == id);
        }

        private void SetCamposAuditoria(EmpleadoExperiencium record, bool bNewRecord)
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
