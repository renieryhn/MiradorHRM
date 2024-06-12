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
using static PlanillaPM.Models.Impuesto;

namespace PlanillaPM.Controllers
{
    public class ImpuestoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ImpuestoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Impuesto
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<Impuesto> registros;
            if (filter != null)
            {
                registros = await _context.Impuestos.Where(r => r.NombreImpuesto.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Impuestos.OrderBy(r => r.Orden).ToListAsync();
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
         public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<Impuesto>? data = null;
             if (data == null)
             {
                data = _context.Impuestos.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "Impuestos.xlsx";
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
        // GET: Impuesto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var impuesto = await _context.Impuestos
                .FirstOrDefaultAsync(m => m.IdImpuesto == id);
            if (impuesto == null)
            {
                return NotFound();
            }

            return View(impuesto);
        }

        // GET: Impuesto/Create
        public IActionResult Create()
        {
            // Obtener el último valor del campo Orden
            int maxOrden = _context.Ingresos.Max(i => (int?)i.Orden) ?? 0;

            // Asignar el próximo valor de Orden al ViewBag
            ViewBag.NextOrden = maxOrden + 1;
            ViewBag.TipoImpuesto = Enum.GetValues(typeof(TipoImpuesto));
            return View();
        }

        // POST: Impuesto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdImpuesto,NombreImpuesto,Tipo,Monto,Formula,Orden,AsignacionAutomatica,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Impuesto impuesto)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(impuesto, true);
                _context.Add(impuesto);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            return View(impuesto);
        }

        // GET: Impuesto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var impuesto = await _context.Impuestos.FindAsync(id);
            if (impuesto == null)
            {
                return NotFound();
            }
            ViewBag.TipoImpuesto = Enum.GetValues(typeof(Impuesto.TipoImpuesto));
            return View(impuesto);
        }

        // POST: Impuesto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdImpuesto,NombreImpuesto,Tipo,Monto,Formula,Orden,AsignacionAutomatica,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Impuesto impuesto)
        {
            if (id != impuesto.IdImpuesto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(impuesto, false);
                    _context.Update(impuesto);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImpuestoExists(impuesto.IdImpuesto))
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
            return View(impuesto);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateOrder(int[] order)
        {
            for (int i = 0; i < order.Length; i++)
            {
                var impuesto = await _context.Impuestos.FindAsync(order[i]);
                if (impuesto != null)
                {
                    impuesto.Orden = i + 1; // Actualiza el campo Orden según el nuevo orden
                    _context.Entry(impuesto).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: Impuesto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var impuesto = await _context.Impuestos
                .FirstOrDefaultAsync(m => m.IdImpuesto == id);
            if (impuesto == null)
            {
                return NotFound();
            }

            return View(impuesto);
        }

        // POST: Impuesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var impuesto = await _context.Impuestos.FindAsync(id);
            try
            {
               
                if (impuesto != null)
                {
                    _context.Impuestos.Remove(impuesto);
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
                return View(impuesto);
            }

        }

        private bool ImpuestoExists(int id)
        {
            return _context.Impuestos.Any(e => e.IdImpuesto == id);
        }
        
        private void SetCamposAuditoria(Impuesto record, bool bNewRecord)
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
