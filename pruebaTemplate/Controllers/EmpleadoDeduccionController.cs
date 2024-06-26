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
using static PlanillaPM.Models.EmpleadoDeduccion;


namespace PlanillaPM.Controllers
{
    public class EmpleadoDeduccionController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoDeduccionController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoDeduccion
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<EmpleadoDeduccion> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoDeduccions.Where(r => r.IdDeduccionNavigation.NombreDeduccion.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoDeduccions.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.EmpleadoDeduccions.ToListAsync();
            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            var IdDeduccionNavigation = await _context.Deduccions.ToListAsync();
            return View(data);
        }
         public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<EmpleadoDeduccion>? data = null;
             if (data == null)
             {
                data = _context.EmpleadoDeduccions.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "EmpleadoDeduccions.xlsx";
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
        // GET: EmpleadoDeduccion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoDeduccion = await _context.EmpleadoDeduccions
                .Include(e => e.IdDeduccionNavigation)
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoDeduccion == id);
            if (empleadoDeduccion == null)
            {
                return NotFound();
            }

            return View(empleadoDeduccion);
        }

        // GET: EmpleadoDeduccion/Create
        public IActionResult Create()
        {
            ViewBag.TipoEstado = Enum.GetValues(typeof(TipoEstado))
                             .Cast<TipoEstado>()
                             .Select(e => new SelectListItem
                             {
                                 Value = ((int)e).ToString(),
                                 Text = e.ToString()
                             }).ToList();
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion");
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");          
            return View();
        }

        public JsonResult GetIngresoDetails(int id)
        {

            var deduccion = _context.Deduccions
                .Where(i => i.IdDeduccion == id)
                .Select(i => new
                {
                    montoIngreso = i.Monto,
                    formulaIngreso = i.Formula,
                    ordenIngreso = i.Orden,
                    tipoIngreso = i.TipoDeduccion

                })
                .FirstOrDefault();

            if (deduccion == null)
            {
                return Json(new { error = "Ingreso no encontrado" });
            }

            return Json(deduccion);
        }

        // POST: EmpleadoDeduccion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoDeduccion,IdEmpleado,IdDeduccion,Tipo,Monto,Formula,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoDeduccion empleadoDeduccion)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(empleadoDeduccion, true);
                _context.Add(empleadoDeduccion);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoDeduccion.IdEmpleado}?tab=settings");
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", empleadoDeduccion.IdDeduccion);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoDeduccion.IdEmpleado);
            return View(empleadoDeduccion);
        }

        // GET: EmpleadoDeduccion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoDeduccion = await _context.EmpleadoDeduccions.FindAsync(id);
            if (empleadoDeduccion == null)
            {
                return NotFound();
            }
            ViewBag.TipoEstado = Enum.GetValues(typeof(EmpleadoDeduccion.TipoEstado));
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", empleadoDeduccion.IdDeduccion);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoDeduccion.IdEmpleado);
            return View(empleadoDeduccion);
        }

        // POST: EmpleadoDeduccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoDeduccion,IdEmpleado,IdDeduccion,Tipo,Monto,Formula,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoDeduccion empleadoDeduccion)
        {
            if (id != empleadoDeduccion.IdEmpleadoDeduccion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(empleadoDeduccion, false);
                    _context.Update(empleadoDeduccion);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoDeduccionExists(empleadoDeduccion.IdEmpleadoDeduccion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoDeduccion.IdEmpleado}?tab=settings");
            }            
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", empleadoDeduccion.IdDeduccion);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoDeduccion.IdEmpleado);
            return View(empleadoDeduccion);
        }

        // GET: EmpleadoDeduccion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoDeduccion = await _context.EmpleadoDeduccions
                .Include(e => e.IdDeduccionNavigation)
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoDeduccion == id);
            if (empleadoDeduccion == null)
            {
                return NotFound();
            }

            return View(empleadoDeduccion);
        }

        // POST: EmpleadoDeduccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var empleadoDeduccion = await _context.EmpleadoDeduccions.FindAsync(id);
            try
            {
               
                if (empleadoDeduccion != null)
                {
                    _context.EmpleadoDeduccions.Remove(empleadoDeduccion);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido eliminado exitosamente.";
                    return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoDeduccion.IdEmpleado}?tab=settings");
                } 
                else
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                    return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoDeduccion.IdEmpleado}?tab=settings");
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
                return View(empleadoDeduccion);
            }

        }

        private bool EmpleadoDeduccionExists(int id)
        {
            return _context.EmpleadoDeduccions.Any(e => e.IdEmpleadoDeduccion == id);
        }
        
        private void SetCamposAuditoria(EmpleadoDeduccion record, bool bNewRecord)
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
