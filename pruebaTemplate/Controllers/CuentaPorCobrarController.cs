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
using static PlanillaPM.Models.CuentaPorCobrar;


namespace PlanillaPM.Controllers
{
    public class CuentaPorCobrarController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public CuentaPorCobrarController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CuentaPorCobrar

        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, string? idDeduccion, int? estado)
        {
            IQueryable<CuentaPorCobrar> query = _context.CuentaPorCobrars;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdDeduccionNavigation.NombreDeduccion.ToLower().Contains(filter.ToLower()));
            }
            if (!String.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdEmpleado.ToString().Contains(idEmpleado));
            }
            if (!String.IsNullOrEmpty(idDeduccion))
            {
                query = query.Where(r => r.IdDeduccion.ToString().Contains(idDeduccion));
            }

            if (estado.HasValue)
            {
                if (estado == 1)
                {
                    query = query.Where(r => r.Activo == false);
                }
                else if (estado == 0)
                {
                    query = query.Where(r => r.Activo == true);
                }
                // No hace falta ningún filtro si el estado es null o no es 0 ni 1 (es decir, se quieren mostrar todos los registros)
            }

            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentIdEmpleado = idEmpleado;
            ViewBag.CurrentIdidDeduccion = idDeduccion;
            ViewBag.CurrentEstado = estado;


            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = query.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = query.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            if (idEmpleado != null)
            {
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            }
            else
            {
                ViewData["IdEmpleado"] = new SelectList(IdEmpleadoNavigation, "IdEmpleado", "NombreCompleto");
            }
         
          
            var IdDeduccionNavigation = await _context.Deduccions.ToListAsync();
            ViewBag.IdDeduccion = new SelectList(IdDeduccionNavigation, "IdDeduccion", "NombreDeduccion");


            return View(data);

        }

        public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<CuentaPorCobrar>? data = null;
             if (data == null)
             {
                data = _context.CuentaPorCobrars.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "CuentaPorCobrars.xlsx";
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
        // GET: CuentaPorCobrar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentaPorCobrar = await _context.CuentaPorCobrars
                .Include(c => c.IdDeduccionNavigation)
                .Include(c => c.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdCuentaPorCobrar == id);
            if (cuentaPorCobrar == null)
            {
                return NotFound();
            }

            return View(cuentaPorCobrar);
        }

        // GET: CuentaPorCobrar/Create
        public IActionResult Create()
        {
            ViewBag.Aprobacion = Enum.GetValues(typeof(Aprobacion));

            ViewBag.CuentaPorCobrarEst = Enum.GetValues(typeof(CuentaPorCobrarEst))
                                             .Cast<CuentaPorCobrarEst>()
                                             .Select(e => new SelectListItem
                                             {
                                                 Value = ((int)e).ToString(),
                                                 Text = e.GetDisplayName()
                                             })
                                             .ToList();

            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion");
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            return View();
        }

        // POST: CuentaPorCobrar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCuentaPorCobrar,IdEmpleado,Descripcion,IdDeduccion,FechaInicio,Monto,InteresAplicado,NumeroPagos,FechaFinalizacion,EstadoCuentaPorCobrar,EstadoAprobacion,AprobadoPor,ComentarioAprobacion,Activo")] CuentaPorCobrar cuentaPorCobrar)
        {
           

            if (ModelState.IsValid)
            {
                SetCamposAuditoria(cuentaPorCobrar, true);
                _context.Add(cuentaPorCobrar);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", cuentaPorCobrar.IdDeduccion);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", cuentaPorCobrar.IdEmpleado);
            return View(cuentaPorCobrar);
        }

        // GET: CuentaPorCobrar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentaPorCobrar = await _context.CuentaPorCobrars.FindAsync(id);
            if (cuentaPorCobrar == null)
            {
                return NotFound();
            }
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", cuentaPorCobrar.IdDeduccion);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", cuentaPorCobrar.IdEmpleado);
            ViewBag.Aprobacion = Enum.GetValues(typeof(CuentaPorCobrar.Aprobacion));

            // Obtener lista de MetodoCalculoEstado
            ViewBag.CuentaPorCobrarEst = Enum.GetValues(typeof(CuentaPorCobrarEst))
                                              .Cast<CuentaPorCobrarEst>()
                                              .Select(e => new SelectListItem
                                              {
                                                  Value = ((int)e).ToString(),
                                                  Text = e.GetDisplayName(),
                                                  Selected = e == cuentaPorCobrar.EstadoCuentaPorCobrar // Seleccionar el valor actual
                                              })
                                              .ToList();

            return View(cuentaPorCobrar);
        }

        // POST: CuentaPorCobrar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCuentaPorCobrar,IdEmpleado,Descripcion,IdDeduccion,FechaInicio,Monto,InteresAplicado,NumeroPagos,FechaFinalizacion,EstadoCuentaPorCobrar,EstadoAprobacion,AprobadoPor,ComentarioAprobacion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] CuentaPorCobrar cuentaPorCobrar)
        {
            if (id != cuentaPorCobrar.IdCuentaPorCobrar)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(cuentaPorCobrar, false);
                    _context.Update(cuentaPorCobrar);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuentaPorCobrarExists(cuentaPorCobrar.IdCuentaPorCobrar))
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
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", cuentaPorCobrar.IdDeduccion);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", cuentaPorCobrar.IdEmpleado);
            
            ViewBag.CuentaPorCobrarEst = Enum.GetValues(typeof(CuentaPorCobrarEst))
                                              .Cast<CuentaPorCobrarEst>()
                                              .Select(e => new SelectListItem
                                              {
                                                  Value = ((int)e).ToString(),
                                                  Text = e.GetDisplayName(),
                                                  Selected = e == cuentaPorCobrar.EstadoCuentaPorCobrar // Seleccionar el valor actual
                                              })
                                              .ToList();

            return View(cuentaPorCobrar);
        }

        // GET: CuentaPorCobrar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentaPorCobrar = await _context.CuentaPorCobrars
                .Include(c => c.IdDeduccionNavigation)
                .Include(c => c.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdCuentaPorCobrar == id);
            if (cuentaPorCobrar == null)
            {
                return NotFound();
            }

            return View(cuentaPorCobrar);
        }

        // POST: CuentaPorCobrar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var cuentaPorCobrar = await _context.CuentaPorCobrars.FindAsync(id);
            try
            {
               
                if (cuentaPorCobrar != null)
                {
                    _context.CuentaPorCobrars.Remove(cuentaPorCobrar);
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
                return View(cuentaPorCobrar);
            }

        }

        private bool CuentaPorCobrarExists(int id)
        {
            return _context.CuentaPorCobrars.Any(e => e.IdCuentaPorCobrar == id);
        }

        private void SetCamposAuditoria(CuentaPorCobrar record, bool bNewRecord)
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
                if (record.FechaCreacion.ToString() == "1/1/0001 00:00:00")
                {
                    record.FechaCreacion = now;
                }
                if (record.CreadoPor == null)
                {
                    record.CreadoPor = CurrentUser;
                }
            }
        }
    }
}
