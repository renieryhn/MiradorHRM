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
using static PlanillaPM.Models.EmpleadoIngreso;

using PlanillaPM.Models;

namespace PlanillaPM.Controllers
{
    public class EmpleadoIngresoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoIngresoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoIngreso
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<EmpleadoIngreso> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoIngresos.Where(r => r.ModificadoPor.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoIngresos.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.EmpleadoIngresos.Include(e => e.IdEmpleadoNavigation).Include(e => e.IdIngresoNavigation);

            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            var IdIngresoNavigation = await _context.EmpleadoIngresos.ToListAsync();
            return View(data);
        }
         public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<EmpleadoIngreso>? data = null;
             if (data == null)
             {
                data = _context.EmpleadoIngresos.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "EmpleadoIngresos.xlsx";
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
        // GET: EmpleadoIngreso/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoIngreso = await _context.EmpleadoIngresos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdIngresoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoIngreso == id);
            if (empleadoIngreso == null)
            {
                return NotFound();
            }

            return View(empleadoIngreso);
        }

       


        // GET: EmpleadoIngreso/Create
        public IActionResult Create()
        {
            ViewBag.TipoEstado = Enum.GetValues(typeof(TipoEstado))
                             .Cast<TipoEstado>()
                             .Select(e => new SelectListItem
                             {
                                 Value = ((int)e).ToString(),
                                 Text = e.ToString()
                             }).ToList();
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            ViewData["IdIngreso"] = new SelectList(_context.Ingresos, "IdIngreso", "NombreIngreso");

            return View();
        }


        public JsonResult GetIngresoDetails(int id)
        {

            var ingreso = _context.Ingresos
                .Where(i => i.IdIngreso == id)
                .Select(i => new
                {
                    montoIngreso = i.Monto,
                    formulaIngreso = i.Formula,
                    ordenIngreso = i.Orden,
                    tipoIngreso = i.TipoIngreso

                })
                .FirstOrDefault();

            if (ingreso == null)
            {
                return Json(new { error = "Ingreso no encontrado" });
            }

            return Json(ingreso);
        }
        // POST: EmpleadoIngreso/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoIngreso,IdIngreso,IdEmpleado,Tipo,Monto,Formula,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoIngreso empleadoIngreso)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(empleadoIngreso, true);
                _context.Add(empleadoIngreso);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoIngreso.IdEmpleado}?tab=profile");
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoIngreso.IdEmpleado);
            ViewData["IdIngreso"] = new SelectList(_context.Ingresos, "IdIngreso", "NombreIngreso", empleadoIngreso.IdIngreso);
            return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoIngreso.IdEmpleado}?tab=profile");
        }

        // GET: EmpleadoIngreso/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoIngreso = await _context.EmpleadoIngresos.FindAsync(id);
            if (empleadoIngreso == null)
            {
                return NotFound();
            }
            ViewBag.TipoEstado = Enum.GetValues(typeof(EmpleadoIngreso.TipoEstado));
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoIngreso.IdEmpleado);
            ViewData["IdIngreso"] = new SelectList(_context.Ingresos, "IdIngreso", "NombreIngreso", empleadoIngreso.IdIngreso);
            return View(empleadoIngreso);
        }

        // POST: EmpleadoIngreso/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoIngreso,IdIngreso,IdEmpleado,Tipo,Monto,Formula,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoIngreso empleadoIngreso)
        {
            if (id != empleadoIngreso.IdEmpleadoIngreso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(empleadoIngreso, false);
                    _context.Update(empleadoIngreso);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoIngresoExists(empleadoIngreso.IdEmpleadoIngreso))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoIngreso.IdEmpleado}?tab=profile");
            }            
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoIngreso.IdEmpleado);
            ViewData["IdIngreso"] = new SelectList(_context.Ingresos, "IdIngreso", "NombreIngreso", empleadoIngreso.IdIngreso);
            return View(empleadoIngreso);
        }

        // GET: EmpleadoIngreso/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoIngreso = await _context.EmpleadoIngresos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdIngresoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoIngreso == id);
            if (empleadoIngreso == null)
            {
                return NotFound();
            }

            return View(empleadoIngreso);
        }

        // POST: EmpleadoIngreso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var empleadoIngreso = await _context.EmpleadoIngresos.FindAsync(id);
            try
            {
               
                if (empleadoIngreso != null)
                {
                    _context.EmpleadoIngresos.Remove(empleadoIngreso);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido eliminado exitosamente.";
                    return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoIngreso.IdEmpleado}?tab=profile");
                } 
                else
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                    return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoIngreso.IdEmpleado}?tab=profile");
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
                return View(empleadoIngreso);
            }

        }

        private bool EmpleadoIngresoExists(int id)
        {
            return _context.EmpleadoIngresos.Any(e => e.IdEmpleadoIngreso == id);
        }
        
        private void SetCamposAuditoria(EmpleadoIngreso record, bool bNewRecord)
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
