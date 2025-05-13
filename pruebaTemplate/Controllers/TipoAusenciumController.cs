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
    public class TipoAusenciumController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public TipoAusenciumController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TipoAusencium
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            ViewBag.Filter = filter;
            List<TipoAusencium> registros;
            if (filter != null)
            {
                registros = await _context.TipoAusencia.Where(r => r.NombreTipoAusencia.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.TipoAusencia.ToListAsync();
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
            var query = _context.TipoAusencia.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(ta => EF.Functions.Like(ta.NombreTipoAusencia, $"%{filter}%"));
            }

            var data = query
                                .Select(ta => new
                        {
                            ta.IdTipoAusencia,
                            NombreTipoAusencia = ta.NombreTipoAusencia,
                            GoseSueldo = ta.GoseSueldo ? "Sí" : "No",
                            Activo = ta.Activo ? "Sí" : "No"
                          
                        })
                        .ToList();

            // Verificar si la lista está vacía
            if (!data.Any())
            {
                TempData["error"] = "No se encontraron tipos de ausencia.";
                return RedirectToAction(nameof(Index));
            }

            // Convertir la lista en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = "TipoAusencia.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Tipo de Ausencia");
                worksheet.Columns().AdjustToContents(); // Ajustar el ancho de las columnas automáticamente

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // GET: TipoAusencium/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoAusencium = await _context.TipoAusencia
                .FirstOrDefaultAsync(m => m.IdTipoAusencia == id);
            if (tipoAusencium == null)
            {
                return NotFound();
            }

            return View(tipoAusencium);
        }

        // GET: TipoAusencium/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoAusencium/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoAusencia,NombreTipoAusencia,GoseSueldo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] TipoAusencium tipoAusencium)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(tipoAusencium, true);
                    _context.Add(tipoAusencium);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("Cannot insert duplicate key row"))
                    {
                        TempData["error"] = "Error: El nombre del tipo de ausencia ya está registrado.";
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
            return View(tipoAusencium);
        }

        // GET: TipoAusencium/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoAusencium = await _context.TipoAusencia.FindAsync(id);
            if (tipoAusencium == null)
            {
                return NotFound();
            }
            return View(tipoAusencium);
        }

        // POST: TipoAusencium/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoAusencia,NombreTipoAusencia,GoseSueldo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] TipoAusencium tipoAusencium)
        {
            if (id != tipoAusencium.IdTipoAusencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(tipoAusencium, false);
                    _context.Update(tipoAusencium);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoAusenciumExists(tipoAusencium.IdTipoAusencia))
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
            return View(tipoAusencium);
        }

        // GET: TipoAusencium/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoAusencium = await _context.TipoAusencia
                .FirstOrDefaultAsync(m => m.IdTipoAusencia == id);
            if (tipoAusencium == null)
            {
                return NotFound();
            }

            return View(tipoAusencium);
        }

        // POST: TipoAusencium/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var tipoAusencium = await _context.TipoAusencia.FindAsync(id);
            try
            {
               
                if (tipoAusencium != null)
                {
                    _context.TipoAusencia.Remove(tipoAusencium);
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
                return View(tipoAusencium);
            }

        }

        private bool TipoAusenciumExists(int id)
        {
            return _context.TipoAusencia.Any(e => e.IdTipoAusencia == id);
        }
        
        private void SetCamposAuditoria(TipoAusencium record, bool bNewRecord)
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
