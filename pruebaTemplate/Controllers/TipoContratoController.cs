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
    public class TipoContratoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public TipoContratoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TipoContrato
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<TipoContrato> registros;
            if (filter != null)
            {
                registros = await _context.TipoContratos.Where(r => r.NombreTipoContrato.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.TipoContratos.ToListAsync();
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
             List<TipoContrato>? data = null;
             if (data == null)
             {
                data = _context.TipoContratos.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "TipoContratos.xlsx";
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
        // GET: TipoContrato/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoContrato = await _context.TipoContratos
                .FirstOrDefaultAsync(m => m.IdTipoContrato == id);
            if (tipoContrato == null)
            {
                return NotFound();
            }

            return View(tipoContrato);
        }

        // GET: TipoContrato/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoContrato/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoContrato,NombreTipoContrato,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] TipoContrato tipoContrato)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(tipoContrato, true);
                    _context.Add(tipoContrato);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("Cannot insert duplicate key row"))
                    {
                        TempData["error"] = "Error: El nombre del tipo de contrato ya está registrado.";
                    }
                    else
                    {
                        TempData["error"] = "Error: No se pudo guardar el registro. Inténtalo de nuevo.";
                    }
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            return View(tipoContrato);
        }

        // GET: TipoContrato/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoContrato = await _context.TipoContratos.FindAsync(id);
            if (tipoContrato == null)
            {
                return NotFound();
            }
            return View(tipoContrato);
        }

        // POST: TipoContrato/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoContrato,NombreTipoContrato,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] TipoContrato tipoContrato)
        {
            if (id != tipoContrato.IdTipoContrato)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(tipoContrato, false);
                    _context.Update(tipoContrato);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoContratoExists(tipoContrato.IdTipoContrato))
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
            return View(tipoContrato);
        }

        // GET: TipoContrato/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoContrato = await _context.TipoContratos
                .FirstOrDefaultAsync(m => m.IdTipoContrato == id);
            if (tipoContrato == null)
            {
                return NotFound();
            }

            return View(tipoContrato);
        }

        // POST: TipoContrato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var tipoContrato = await _context.TipoContratos.FindAsync(id);
            try
            {
               
                if (tipoContrato != null)
                {
                    _context.TipoContratos.Remove(tipoContrato);
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
                return View(tipoContrato);
            }

        }

        private bool TipoContratoExists(int id)
        {
            return _context.TipoContratos.Any(e => e.IdTipoContrato == id);
        }
        
        private void SetCamposAuditoria(TipoContrato record, bool bNewRecord)
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
