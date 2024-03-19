using System.Data;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static PlanillaPM.cGeneralFun;

using PlanillaPM.Models;

namespace PlanillaPM.Controllers
{
    public class EmpleadoActivoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoActivoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoActivo
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<EmpleadoActivo> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoActivos.Where(r => r.Model.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoActivos.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.EmpleadoActivos.Include(e => e.IdEmpleadoNavigation).Include(e => e.IdProductoNavigation);
            return View(data);
        }
         public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<EmpleadoActivo>? data = null;
             if (data == null)
             {
                data = _context.EmpleadoActivos.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "EmpleadoActivos.xlsx";
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
        // GET: EmpleadoActivo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoActivo = await _context.EmpleadoActivos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoActivo == id);
            if (empleadoActivo == null)
            {
                return NotFound();
            }

            return View(empleadoActivo);
        }

        // GET: EmpleadoActivo/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado");
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto");
            return View();
        }

        // POST: EmpleadoActivo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoActivo,IdEmpleado,IdProducto,Model,NumeroSerie,Estado,Cantidad,PrecioEstimado,FechaAsignacion,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoActivo empleadoActivo)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(empleadoActivo, true);
                _context.Add(empleadoActivo);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoActivo.IdEmpleado);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", empleadoActivo.IdProducto);
            return View(empleadoActivo);
        }

        // GET: EmpleadoActivo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoActivo = await _context.EmpleadoActivos.FindAsync(id);
            if (empleadoActivo == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoActivo.IdEmpleado);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", empleadoActivo.IdProducto);
            return View(empleadoActivo);
        }

        // POST: EmpleadoActivo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoActivo,IdEmpleado,IdProducto,Model,NumeroSerie,Estado,Cantidad,PrecioEstimado,FechaAsignacion,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoActivo empleadoActivo)
        {
            if (id != empleadoActivo.IdEmpleadoActivo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(empleadoActivo, false);
                    _context.Update(empleadoActivo);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoActivoExists(empleadoActivo.IdEmpleadoActivo))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoActivo.IdEmpleado);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", empleadoActivo.IdProducto);
            return View(empleadoActivo);
        }

        // GET: EmpleadoActivo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoActivo = await _context.EmpleadoActivos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoActivo == id);
            if (empleadoActivo == null)
            {
                return NotFound();
            }

            return View(empleadoActivo);
        }

        // POST: EmpleadoActivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var empleadoActivo = await _context.EmpleadoActivos.FindAsync(id);
            try
            {
               
                if (empleadoActivo != null)
                {
                    _context.EmpleadoActivos.Remove(empleadoActivo);
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
                return View(empleadoActivo);
            }

        }

        private bool EmpleadoActivoExists(int id)
        {
            return _context.EmpleadoActivos.Any(e => e.IdEmpleadoActivo == id);
        }

        private void SetCamposAuditoria(EmpleadoActivo record, bool bNewRecord)
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
