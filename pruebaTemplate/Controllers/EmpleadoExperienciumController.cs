using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;

namespace PlanillaPM.Controllers
{
    public class EmpleadoExperienciumController : Controller
    {
        private readonly PlanillaContext _context;

        public EmpleadoExperienciumController(PlanillaContext context)
        {
            _context = context;
        }

        // GET: EmpleadoExperiencium
        public async Task<IActionResult> Index()
        {
            var planillaContext = _context.EmpleadoExperiencia.Include(e => e.IdEmpleadoNavigation);
            return View(await planillaContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("IdEmpleadoExperiencia,IdEmpleado,Empresa,Cargo,FechaDesde,FechaHasta,Descripcion,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoExperiencium empleadoExperiencium)
        {
            try
            {
                if (ModelState.IsValid)
                {
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
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoExperiencia,IdEmpleado,Empresa,Cargo,FechaDesde,FechaHasta,Descripcion,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoExperiencium empleadoExperiencium)
        {
            try
            {
                if (id != empleadoExperiencium.IdEmpleadoExperiencia)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
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
    }
}
