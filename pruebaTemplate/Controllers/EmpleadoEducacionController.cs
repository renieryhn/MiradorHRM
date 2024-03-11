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
    public class EmpleadoEducacionController : Controller
    {
        private readonly PlanillaContext _context;

        public EmpleadoEducacionController(PlanillaContext context)
        {
            _context = context;
        }

        // GET: EmpleadoEducacion
        public async Task<IActionResult> Index()
        {
            var planillaContext = _context.EmpleadoEducacions.Include(e => e.IdEmpleadoNavigation);
            return View(await planillaContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("IdEmpleadoEducacion,IdEmpleado,Institucion,TituloObtenido,FechaDesde,FechaHasta,Comentarios,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoEducacion empleadoEducacion)
        {
            try
            {
                if (ModelState.IsValid)
                {
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
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoEducacion,IdEmpleado,Institucion,TituloObtenido,FechaDesde,FechaHasta,Comentarios,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoEducacion empleadoEducacion)
        {
            try
            {
                if (id != empleadoEducacion.IdEmpleadoEducacion)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
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
    }
}
