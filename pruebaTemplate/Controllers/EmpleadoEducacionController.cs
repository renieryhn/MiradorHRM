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
    public class EmpleadoEducacionController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoEducacionController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoEducacion
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            //var planillaContext = _context.EmpleadoEducacions.Include(e => e.IdEmpleadoNavigation);
            //return View(await planillaContext.ToListAsync());
            List<EmpleadoEducacion> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoEducacions.Where(r => r.Institucion.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoEducacions.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();

            return View(data);

        }

        public ActionResult Download()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            List<EmpleadoEducacion>? data = null;
            if (data == null)
            {
                data = _context.EmpleadoEducacions.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "EmpleadoEducacion.xlsx";
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

        // GET: EmpleadoEducacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoEducacion = await _context.EmpleadoEducacions
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoEducacion == id);
            if (empleadoEducacion == null)
            {
                return NotFound();
            }

            return View(empleadoEducacion);
        }

        // GET: EmpleadoEducacion/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado");
            return View();
        }

        // POST: EmpleadoEducacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoEducacion,IdEmpleado,Institucion,TituloObtenido,FechaDesde,FechaHasta,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoEducacion empleadoEducacion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoEducacion, true);
                    _context.Add(empleadoEducacion);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["mensaje"] = "El registro se creó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoEducacion.IdEmpleado);

                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, verifique los datos.";

                return View(empleadoEducacion);
            }
            catch (Exception ex)
            {
                
                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, intente nuevamente.";

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: EmpleadoEducacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoEducacion = await _context.EmpleadoEducacions.FindAsync(id);
            if (empleadoEducacion == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoEducacion.IdEmpleado);
            return View(empleadoEducacion);
        }

        // POST: EmpleadoEducacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoEducacion,IdEmpleado,Institucion,TituloObtenido,FechaDesde,FechaHasta,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoEducacion empleadoEducacion)
        {
            try
            {
                if (id != empleadoEducacion.IdEmpleadoEducacion)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoEducacion, false);
                    _context.Update(empleadoEducacion);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["mensaje"] = "El registro se actualizó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoEducacion.IdEmpleado);

                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar actualizar el registro. Por favor, verifique los datos.";

                return View(empleadoEducacion);
            }
            catch (DbUpdateConcurrencyException)
            {
               
                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar actualizar el registro. Por favor, intente nuevamente.";

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: EmpleadoEducacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoEducacion = await _context.EmpleadoEducacions
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoEducacion == id);
            if (empleadoEducacion == null)
            {
                return NotFound();
            }

            return View(empleadoEducacion);
        }

        // POST: EmpleadoEducacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var empleadoEducacion = await _context.EmpleadoEducacions.FindAsync(id);
                if (empleadoEducacion != null)
                {
                    _context.EmpleadoEducacions.Remove(empleadoEducacion);
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

        private bool EmpleadoEducacionExists(int id)
        {
            return _context.EmpleadoEducacions.Any(e => e.IdEmpleadoEducacion == id);
        }
        private void SetCamposAuditoria(EmpleadoEducacion record, bool bNewRecord)
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
