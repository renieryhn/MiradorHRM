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
    public class EmpleadoContratoController : Controller
    {
        private readonly PlanillaContext _context;

        public EmpleadoContratoController(PlanillaContext context)
        {
            _context = context;
        }

        // GET: EmpleadoContrato
        public async Task<IActionResult> Index()
        {
            var planillaContext = _context.EmpleadoContratos.Include(e => e.IdCargoNavigation).Include(e => e.IdEmpleadoNavigation);
            return View(await planillaContext.ToListAsync());
        }

        // GET: EmpleadoContrato/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoContrato = await _context.EmpleadoContratos
                .Include(e => e.IdCargoNavigation)
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoContrato == id);
            if (empleadoContrato == null)
            {
                return NotFound();
            }

            return View(empleadoContrato);
        }

        // GET: EmpleadoContrato/Create
        public IActionResult Create()
        {
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "NombreCargo");
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato");
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado");
            return View();
        }

        // POST: EmpleadoContrato/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoContrato,IdEmpleado,CodigoContrato,IdTipoContrato,IdCargo,Estado,VigenciaMeses,FechaInicio,FechaFin,Salario,Descripcion,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoContrato empleadoContrato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(empleadoContrato);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["mensaje"] = "El registro se creó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }

                ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "NombreCargo", empleadoContrato.IdCargo);
                ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato", empleadoContrato.IdTipoContrato);
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoContrato.IdEmpleado);

                // Agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, verifique los datos.";

                return View(empleadoContrato);
            }
            catch (Exception ex)
            {
                // Puedes manejar la excepción de manera específica o simplemente registrarla
                // Logging.LogError(ex, "Error al intentar crear un registro");

                // Agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, intente nuevamente.";

                return RedirectToAction(nameof(Index));
            }
        }


        // GET: EmpleadoContrato/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoContrato = await _context.EmpleadoContratos.FindAsync(id);
            if (empleadoContrato == null)
            {
                return NotFound();
            }
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "NombreCargo", empleadoContrato.IdCargo);
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato", empleadoContrato.IdTipoContrato);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoContrato.IdEmpleado);
            return View(empleadoContrato);
        }

        // POST: EmpleadoContrato/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoContrato,IdEmpleado,CodigoContrato,IdTipoContrato,IdCargo,Estado,VigenciaMeses,FechaInicio,FechaFin,Salario,Descripcion,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoContrato empleadoContrato)
        {
            try
            {
                if (id != empleadoContrato.IdEmpleadoContrato)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    _context.Update(empleadoContrato);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["mensaje"] = "El registro se actualizó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }

                ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "NombreCargo", empleadoContrato.IdCargo);
                ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato", empleadoContrato.IdTipoContrato);
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoContrato.IdEmpleado);

                // Agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar actualizar el registro. Por favor, verifique los datos.";

                return View(empleadoContrato);
            }
            catch (DbUpdateConcurrencyException)
            {

                // Agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar actualizar el registro. Por favor, intente nuevamente.";

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: EmpleadoContrato/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoContrato = await _context.EmpleadoContratos
                .Include(e => e.IdCargoNavigation)
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoContrato == id);
            if (empleadoContrato == null)
            {
                return NotFound();
            }

            return View(empleadoContrato);
        }

        // POST: EmpleadoContrato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var empleadoContrato = await _context.EmpleadoContratos.FindAsync(id);

                if (empleadoContrato != null)
                {
                    _context.EmpleadoContratos.Remove(empleadoContrato);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["Success"] = "El registro se eliminó exitosamente.";

                    return RedirectToAction(nameof(Index));
                }

                // Agregar mensaje de error a TempData
                TempData["Error"] = "No se encontró el registro que intenta eliminar. Por favor, verifique.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Puedes manejar la excepción de manera específica o simplemente registrarla
                // Logging.LogError(ex, "Error al intentar eliminar un registro");

                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar eliminar el registro. Por favor, intente nuevamente.";

                return RedirectToAction(nameof(Index));
            }
        }

        private bool EmpleadoContratoExists(int id)
        {
            return _context.EmpleadoContratos.Any(e => e.IdEmpleadoContrato == id);
        }
    }
}
