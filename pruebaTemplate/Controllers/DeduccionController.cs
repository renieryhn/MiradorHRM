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
using static PlanillaPM.Models.Deduccion;

namespace PlanillaPM.Controllers
{
    public class DeduccionController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public DeduccionController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Deduccion
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            ViewBag.Filter = filter;
            List<Deduccion> registros;
            if (filter != null)
            {
                registros = await _context.Deduccions.Where(r => r.NombreDeduccion.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {              
                registros = await _context.Deduccions.OrderBy(r => r.Orden).ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }
        public ActionResult Download(string? filter)
        {
            var query = _context.Deduccions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(d => EF.Functions.Like(d.NombreDeduccion, $"%{filter}%"));
            }

            var data = query
                .OrderBy(d => d.Orden)
                .Select(d => new
                {
                    d.IdDeduccion,
                    d.NombreDeduccion,
                    MetodoCalculo = d.MetodoCalculo.ToString(), 
                    TipoDeduccion = d.TipoDeduccion.ToString(), 
                    TipoCalculo = d.TipoCalculo.ToString(),
                    d.Monto,
                    d.Formula,
                    d.Orden,
                    DeducibleImpuesto = d.DeducibleImpuesto ? "Sí" : "No",
                    BasadoEnTodo = d.BasadoEnTodo ? "Sí" : "No",
                    AsignacionAutomatica = d.AsignacionAutomatica ? "Sí" : "No", 
                    Activo = d.Activo ? "Sí" : "No" 
                   
                })
                .ToList();

            // Verificar si la lista está vacía
            if (!data.Any())
            {
                TempData["error"] = "No se encontraron Registros.";
                return RedirectToAction(nameof(Index));
            }

            // Convertir la lista a DataTable
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            string fileName = "Deducciones.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Añadir la tabla al workbook con un nombre significativo para la hoja
                wb.Worksheets.Add(table, "Deducciones");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // GET: Deduccion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deduccion = await _context.Deduccions
                .FirstOrDefaultAsync(m => m.IdDeduccion == id);
            if (deduccion == null)
            {
                return NotFound();
            }

            return View(deduccion);
        }

        // GET: Deduccion/Create
        public IActionResult Create()
        {
            // Obtener el último valor del campo Orden
            int maxOrden = _context.Deduccions.Max(i => (int?)i.Orden) ?? 0;

            // Asignar el próximo valor de Orden al ViewBag
            ViewBag.NextOrden = maxOrden + 1;

            ViewBag.TipoCalculoEstado = Enum.GetValues(typeof(TipoCalculoEstado));
            // Obtener lista de MetodoCalculoEstado
            ViewBag.MetodoCalculoEstado = Enum.GetValues(typeof(MetodoCalculoEstado))
                                              .Cast<MetodoCalculoEstado>()
                                              .Select(e => new SelectListItem
                                              {
                                                  Value = ((int)e).ToString(),
                                                  Text = e.GetDisplayName()
                                              })
                                              .ToList();

            // Obtener lista de TipoDeduccionEstado
            ViewBag.TipoDeduccionEstado = Enum.GetValues(typeof(TipoDeduccionEstado))
                                              .Cast<TipoDeduccionEstado>()
                                              .Select(e => new SelectListItem
                                              {
                                                  Value = ((int)e).ToString(),
                                                  Text = e.GetDisplayName()
                                              })
                                              .ToList();
            return View();
        }

        // POST: Deduccion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDeduccion,NombreDeduccion,MetodoCalculo,TipoDeduccion,TipoCalculo,Monto,Formula,Orden,DeducibleImpuesto,BasadoEnTodo,AsignacionAutomatica,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Deduccion deduccion)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(deduccion, true);
                _context.Add(deduccion);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            return View(deduccion);
        }

        // GET: Deduccion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deduccion = await _context.Deduccions.FindAsync(id);
            if (deduccion == null)
            {
                return NotFound();
            }
            ViewBag.TipoCalculoEstado = Enum.GetValues(typeof(Deduccion.TipoCalculoEstado));
            // Obtener lista de MetodoCalculoEstado
            ViewBag.MetodoCalculoEstado = Enum.GetValues(typeof(MetodoCalculoEstado))
                                              .Cast<MetodoCalculoEstado>()
                                              .Select(e => new SelectListItem
                                              {
                                                  Value = ((int)e).ToString(),
                                                  Text = e.GetDisplayName(),
                                                  Selected = e == deduccion.MetodoCalculo // Seleccionar el valor actual
                                              })
                                              .ToList();

            // Obtener lista de TipoDeduccionEstado
            ViewBag.TipoDeduccionEstado = Enum.GetValues(typeof(TipoDeduccionEstado))
                                              .Cast<TipoDeduccionEstado>()
                                              .Select(e => new SelectListItem
                                              {
                                                  Value = ((int)e).ToString(),
                                                  Text = e.GetDisplayName(),
                                                  Selected = e == deduccion.TipoDeduccion // Seleccionar el valor actual
                                              })
                                              .ToList();
            return View(deduccion);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(int[] order)
        {
            for (int i = 0; i < order.Length; i++)
            {
                var deduccion = await _context.Deduccions.FindAsync(order[i]);
                if (deduccion != null)
                {
                    deduccion.Orden = i + 1; // Actualiza el campo Orden según el nuevo orden
                    _context.Entry(deduccion).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: Deduccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDeduccion,NombreDeduccion,MetodoCalculo,TipoDeduccion,TipoCalculo,Monto,Formula,Orden,DeducibleImpuesto,BasadoEnTodo,AsignacionAutomatica,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Deduccion deduccion)
        {
            if (id != deduccion.IdDeduccion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(deduccion, false);
                    _context.Update(deduccion);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeduccionExists(deduccion.IdDeduccion))
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

            // Re-populate the ViewBag lists in case of a model state error
            ViewBag.MetodoCalculoEstado = Enum.GetValues(typeof(MetodoCalculoEstado))
                                              .Cast<MetodoCalculoEstado>()
                                              .Select(e => new SelectListItem
                                              {
                                                  Value = ((int)e).ToString(),
                                                  Text = e.GetDisplayName(),
                                                  Selected = e == deduccion.MetodoCalculo // Seleccionar el valor actual
                                              })
                                              .ToList();

            ViewBag.TipoDeduccionEstado = Enum.GetValues(typeof(TipoDeduccionEstado))
                                              .Cast<TipoDeduccionEstado>()
                                              .Select(e => new SelectListItem
                                              {
                                                  Value = ((int)e).ToString(),
                                                  Text = e.GetDisplayName(),
                                                  Selected = e == deduccion.TipoDeduccion // Seleccionar el valor actual
                                              })
                                              .ToList();


            return View(deduccion);
        }

        // GET: Deduccion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deduccion = await _context.Deduccions
                .FirstOrDefaultAsync(m => m.IdDeduccion == id);
            if (deduccion == null)
            {
                return NotFound();
            }

            return View(deduccion);
        }

        // POST: Deduccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var deduccion = await _context.Deduccions.FindAsync(id);
            try
            {
               
                if (deduccion != null)
                {
                    _context.Deduccions.Remove(deduccion);
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
                return View(deduccion);
            }

        }

        private bool DeduccionExists(int id)
        {
            return _context.Deduccions.Any(e => e.IdDeduccion == id);
        }
        
        private void SetCamposAuditoria(Deduccion record, bool bNewRecord)
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
