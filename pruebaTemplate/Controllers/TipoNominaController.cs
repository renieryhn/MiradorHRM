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
    public class TipoNominaController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public TipoNominaController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TipoNomina
        public async Task<IActionResult> Index(int pg, string? filter)
        {

            ViewBag.Filter = filter;

            List<TipoNomina> registros;
            if (filter != null)
            {
                registros = await _context.TipoNominas.Where(r => r.NombreTipoNomina.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.TipoNominas.ToListAsync();
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
            var query = _context.TipoNominas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(tn => EF.Functions.Like(tn.NombreTipoNomina, $"%{filter}%"));
            }

            var data = query
                                .Select(tn => new
                        {
                            tn.IdTipoNomina,
                            NombreTipoNomina = tn.NombreTipoNomina,
                            PagadaCadaNdias = tn.PagadaCadaNdias,
                            Activo = tn.Activo ? "Sí" : "No"
                            
                        })
                        .ToList();

            // Verificar si la lista está vacía
            if (!data.Any())
            {
                TempData["error"] = "No se encontraron tipos de nómina.";
                return RedirectToAction(nameof(Index));
            }

            // Convertir la lista en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = "TipoNominas.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Tipos de Nómina");
                worksheet.Columns().AdjustToContents(); // Ajustar el ancho de las columnas automáticamente

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }


        // GET: TipoNomina/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoNomina = await _context.TipoNominas
                .FirstOrDefaultAsync(m => m.IdTipoNomina == id);
            if (tipoNomina == null)
            {
                return NotFound();
            }

            return View(tipoNomina);
        }

        // GET: TipoNomina/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoNomina/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoNomina,NombreTipoNomina,PagadaCadaNdias,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] TipoNomina tipoNomina)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(tipoNomina, true);
                _context.Add(tipoNomina);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            return View(tipoNomina);
        }

        // GET: TipoNomina/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoNomina = await _context.TipoNominas.FindAsync(id);
            if (tipoNomina == null)
            {
                return NotFound();
            }
            return View(tipoNomina);
        }

        // POST: TipoNomina/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoNomina,NombreTipoNomina,PagadaCadaNdias,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] TipoNomina tipoNomina)
        {
            if (id != tipoNomina.IdTipoNomina)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(tipoNomina, false);
                    _context.Update(tipoNomina);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoNominaExists(tipoNomina.IdTipoNomina))
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
            return View(tipoNomina);
        }

        // GET: TipoNomina/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoNomina = await _context.TipoNominas
                .FirstOrDefaultAsync(m => m.IdTipoNomina == id);
            if (tipoNomina == null)
            {
                return NotFound();
            }

            return View(tipoNomina);
        }

        // POST: TipoNomina/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var tipoNomina = await _context.TipoNominas.FindAsync(id);
            try
            {
               
                if (tipoNomina != null)
                {
                    _context.TipoNominas.Remove(tipoNomina);
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
                return View(tipoNomina);
            }

        }

        private bool TipoNominaExists(int id)
        {
            return _context.TipoNominas.Any(e => e.IdTipoNomina == id);
        }
        
        private void SetCamposAuditoria(TipoNomina record, bool bNewRecord)
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
