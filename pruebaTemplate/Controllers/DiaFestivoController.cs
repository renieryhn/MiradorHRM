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
    public class DiaFestivoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public DiaFestivoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetDiasFestivos()
        {
            // Obtener todos los días festivos de la base de datos
            var diasFestivos = _context.DiaFestivos.ToList();

            // Devolver los días festivos en formato JSON
            return Json(diasFestivos);
        }

        // GET: DiaFestivo
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<DiaFestivo> registros;
            if (filter != null)
            {
                registros = await _context.DiaFestivos.Where(r => r.NombreDiaFestivo.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.DiaFestivos.ToListAsync();
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
             List<DiaFestivo>? data = null;
             if (data == null)
             {
                data = _context.DiaFestivos.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "DiaFestivos.xlsx";
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
        // GET: DiaFestivo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diaFestivo = await _context.DiaFestivos
                .FirstOrDefaultAsync(m => m.IdDiaFestivo == id);
            if (diaFestivo == null)
            {
                return NotFound();
            }

            return View(diaFestivo);
        }

        // GET: DiaFestivo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DiaFestivo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDiaFestivo,NombreDiaFestivo,FechaDesde,FechaHasta,Color,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] DiaFestivo diaFestivo)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(diaFestivo, true);
                _context.Add(diaFestivo);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            return View(diaFestivo);
        }

        // GET: DiaFestivo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diaFestivo = await _context.DiaFestivos.FindAsync(id);
            if (diaFestivo == null)
            {
                return NotFound();
            }
            return View(diaFestivo);
        }

        // POST: DiaFestivo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDiaFestivo,NombreDiaFestivo,FechaDesde,FechaHasta,Color,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] DiaFestivo diaFestivo)
        {
            if (id != diaFestivo.IdDiaFestivo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(diaFestivo, false);
                    _context.Update(diaFestivo);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiaFestivoExists(diaFestivo.IdDiaFestivo))
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
            return View(diaFestivo);
        }

        // GET: DiaFestivo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diaFestivo = await _context.DiaFestivos
                .FirstOrDefaultAsync(m => m.IdDiaFestivo == id);
            if (diaFestivo == null)
            {
                return NotFound();
            }

            return View(diaFestivo);
        }

        // POST: DiaFestivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var diaFestivo = await _context.DiaFestivos.FindAsync(id);
            try
            {
               
                if (diaFestivo != null)
                {
                    _context.DiaFestivos.Remove(diaFestivo);
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
                return View(diaFestivo);
            }

        }

        private bool DiaFestivoExists(int id)
        {
            return _context.DiaFestivos.Any(e => e.IdDiaFestivo == id);
        }
        
        private void SetCamposAuditoria(DiaFestivo record, bool bNewRecord)
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
