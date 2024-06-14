using System.Data;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static PlanillaPM.cGeneralFun;
using Microsoft.AspNetCore.Identity;
using PlanillaPM.ViewModel;

using PlanillaPM.Models;

namespace PlanillaPM.Controllers
{
    public class EmpleadoImpuestoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoImpuestoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoImpuesto
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<EmpleadoImpuesto> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoImpuestos.Where(r => r.IdImpuestoNavigation.NombreImpuesto.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoImpuestos.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.EmpleadoImpuestos.Include(e => e.IdEmpleadoNavigation).Include(e => e.IdImpuestoNavigation);

            var IdEmpleadoNavigation = _context.Empleados.ToListAsync();
            var IdImpuestoNavigation = _context.Impuestos.ToListAsync();
            return View(data);
        }
         public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<EmpleadoImpuesto>? data = null;
             if (data == null)
             {
                data = _context.EmpleadoImpuestos.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "EmpleadoImpuestos.xlsx";
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
        // GET: EmpleadoImpuesto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoImpuesto = await _context.EmpleadoImpuestos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdImpuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoImpuesto == id);
            if (empleadoImpuesto == null)
            {
                return NotFound();
            }

            return View(empleadoImpuesto);
        }

        // GET: EmpleadoImpuesto/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto");
            return View();
        }

        // POST: EmpleadoImpuesto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoImpuesto,IdImpuesto,IdEmpleado,Excento,Orden,FechaCreacion,Activo,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoImpuesto empleadoImpuesto)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(empleadoImpuesto, true);
                _context.Add(empleadoImpuesto);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoImpuesto.IdEmpleado);
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto", empleadoImpuesto.IdImpuesto);
            return View(empleadoImpuesto);
        }

        // GET: EmpleadoImpuesto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoImpuesto = await _context.EmpleadoImpuestos.FindAsync(id);
            if (empleadoImpuesto == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoImpuesto.IdEmpleado);
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto", empleadoImpuesto.IdImpuesto);
            return View(empleadoImpuesto);
        }

        // POST: EmpleadoImpuesto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoImpuesto,IdImpuesto,IdEmpleado,Excento,Orden,FechaCreacion,Activo,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoImpuesto empleadoImpuesto)
        {
            if (id != empleadoImpuesto.IdEmpleadoImpuesto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(empleadoImpuesto, false);
                    _context.Update(empleadoImpuesto);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoImpuestoExists(empleadoImpuesto.IdEmpleadoImpuesto))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoImpuesto.IdEmpleado);
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto", empleadoImpuesto.IdImpuesto);
            return View(empleadoImpuesto);
        }

        // GET: EmpleadoImpuesto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoImpuesto = await _context.EmpleadoImpuestos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdImpuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoImpuesto == id);
            if (empleadoImpuesto == null)
            {
                return NotFound();
            }

            return View(empleadoImpuesto);
        }

        // POST: EmpleadoImpuesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var empleadoImpuesto = await _context.EmpleadoImpuestos.FindAsync(id);
            try
            {
               
                if (empleadoImpuesto != null)
                {
                    _context.EmpleadoImpuestos.Remove(empleadoImpuesto);
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
                return View(empleadoImpuesto);
            }

        }

        private bool EmpleadoImpuestoExists(int id)
        {
            return _context.EmpleadoImpuestos.Any(e => e.IdEmpleadoImpuesto == id);
        }
        
        private void SetCamposAuditoria(EmpleadoImpuesto record, bool bNewRecord)
        {
            var now = DateTime.Now;
            var CurrentUser =  _userManager.GetUserName(User);
           
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
