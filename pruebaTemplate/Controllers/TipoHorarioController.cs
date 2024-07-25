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
    public class TipoHorarioController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public TipoHorarioController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TipoHorario
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<TipoHorario> registros;
            if (filter != null)
            {
                registros = await _context.TipoHorarios.Where(r => r.NombreTipoHorario.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.TipoHorarios.ToListAsync();
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
             List<TipoHorario>? data = null;
             if (data == null)
             {
                data = _context.TipoHorarios.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "TipoHorarios.xlsx";
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
        // GET: TipoHorario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHorario = await _context.TipoHorarios
                .FirstOrDefaultAsync(m => m.IdTipoHorario == id);
            if (tipoHorario == null)
            {
                return NotFound();
            }

            return View(tipoHorario);
        }

        // GET: TipoHorario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoHorario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoHorario,NombreTipoHorario,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] TipoHorario tipoHorario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(tipoHorario, true);
                    _context.Add(tipoHorario);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_TipoHorario"))
                    {
                        TempData["error"] = "Error: El nombre del tipo de horario ya está registrado. Por favor, ingrese un nombre diferente.";
                    }
                    else
                    {
                        TempData["error"] = "Error: Ocurrió un error al intentar crear el registro. Por favor, inténtelo de nuevo.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Error: Ocurrió un error inesperado. Por favor, inténtelo de nuevo.";
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            return View(tipoHorario);
        }

        // GET: TipoHorario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHorario = await _context.TipoHorarios.FindAsync(id);
            if (tipoHorario == null)
            {
                return NotFound();
            }
            return View(tipoHorario);
        }

        // POST: TipoHorario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoHorario,NombreTipoHorario,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] TipoHorario tipoHorario)
        {
            if (id != tipoHorario.IdTipoHorario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(tipoHorario, false);
                    _context.Update(tipoHorario);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoHorarioExists(tipoHorario.IdTipoHorario))
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
            return View(tipoHorario);
        }

        // GET: TipoHorario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoHorario = await _context.TipoHorarios
                .FirstOrDefaultAsync(m => m.IdTipoHorario == id);
            if (tipoHorario == null)
            {
                return NotFound();
            }

            return View(tipoHorario);
        }

        // POST: TipoHorario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var tipoHorario = await _context.TipoHorarios.FindAsync(id);
            try
            {
               
                if (tipoHorario != null)
                {
                    _context.TipoHorarios.Remove(tipoHorario);
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
                return View(tipoHorario);
            }

        }

        private bool TipoHorarioExists(int id)
        {
            return _context.TipoHorarios.Any(e => e.IdTipoHorario == id);
        }
        
        private void SetCamposAuditoria(TipoHorario record, bool bNewRecord)
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
