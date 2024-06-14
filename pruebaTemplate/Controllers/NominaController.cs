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
using static PlanillaPM.Models.Viatico;
using static PlanillaPM.Models.Nomina;

namespace PlanillaPM.Controllers
{
    public class NominaController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public NominaController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Nomina
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<Nomina> registros;
            if (filter != null)
            {
                registros = await _context.Nominas.Where(r => r.ModificadoPor.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Nominas.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.Nominas.Include(n => n.IdTipoNominaNavigation);
            var IdTipoNominaNavigation = _context.TipoNominas.ToListAsync();
            return View(data);
        }
         public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<Nomina>? data = null;
             if (data == null)
             {
                data = _context.Nominas.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "Nominas.xlsx";
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
        // GET: Nomina/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomina = await _context.Nominas
                .Include(n => n.IdTipoNominaNavigation)
                .FirstOrDefaultAsync(m => m.IdNomina == id);
            if (nomina == null)
            {
                return NotFound();
            }

            return View(nomina);
        }

        // GET: Nomina/Create
        public IActionResult Create()
        {
            ViewBag.NominaEstado = Enum.GetValues(typeof(NominaEstado));
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas, "IdTipoNomina", "NombreTipoNomina");
            return View();
        }

        // POST: Nomina/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNomina,IdTipoNomina,Comentarios,PeriodoFiscal,Mes,FechaPago,TotalIngresos,TotalDeducciones,TotalImpuestos,TotalEmpleadosEnNomina,PagoNeto,EstadoNomina,AprobadaPor,ComentariosAprobador,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Nomina nomina)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(nomina, true);
                _context.Add(nomina);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas, "IdTipoNomina", "NombreTipoNomina", nomina.IdTipoNomina);
            return View(nomina);
        }

        // GET: Nomina/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomina = await _context.Nominas.FindAsync(id);
            if (nomina == null)
            {
                return NotFound();
            }

            ViewBag.NominaEstado = Enum.GetValues(typeof(Nomina.NominaEstado));
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas, "IdTipoNomina", "NombreTipoNomina", nomina.IdTipoNomina);
            return View(nomina);
        }

        // POST: Nomina/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNomina,IdTipoNomina,Comentarios,PeriodoFiscal,Mes,FechaPago,TotalIngresos,TotalDeducciones,TotalImpuestos,TotalEmpleadosEnNomina,PagoNeto,EstadoNomina,AprobadaPor,ComentariosAprobador,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Nomina nomina)
        {
            if (id != nomina.IdNomina)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(nomina, false);
                    _context.Update(nomina);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NominaExists(nomina.IdNomina))
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
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas, "IdTipoNomina", "NombreTipoNomina", nomina.IdTipoNomina);
            return View(nomina);
        }

        // GET: Nomina/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomina = await _context.Nominas
                .Include(n => n.IdTipoNominaNavigation)
                .FirstOrDefaultAsync(m => m.IdNomina == id);
            if (nomina == null)
            {
                return NotFound();
            }

            return View(nomina);
        }

        // POST: Nomina/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var nomina = await _context.Nominas.FindAsync(id);
            try
            {
               
                if (nomina != null)
                {
                    _context.Nominas.Remove(nomina);
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
                return View(nomina);
            }

        }

        private bool NominaExists(int id)
        {
            return _context.Nominas.Any(e => e.IdNomina == id);
        }
        
        private void SetCamposAuditoria(Nomina record, bool bNewRecord)
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
