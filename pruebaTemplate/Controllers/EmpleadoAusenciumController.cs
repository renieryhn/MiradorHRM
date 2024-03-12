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
    public class EmpleadoAusenciumController : Controller
    {
        private readonly PlanillaContext _context;

        public EmpleadoAusenciumController(PlanillaContext context)
        {
            _context = context;
        }

        // GET: EmpleadoAusencium
        public async Task<IActionResult> Index()
        {
            var planillaContext = _context.EmpleadoAusencia.Include(e => e.IdEmpleadoNavigation).Include(e => e.IdTipoAusenciaNavigation);
            return View(await planillaContext.ToListAsync());
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
    }
}
