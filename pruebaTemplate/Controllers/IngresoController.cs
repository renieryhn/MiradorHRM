using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using Syncfusion.DocIO.DLS;
using System.Data;
using static PlanillaPM.cGeneralFun;
using static PlanillaPM.Models.Impuesto;
using static PlanillaPM.Models.Ingreso;

namespace PlanillaPM.Controllers
{
    public class IngresoController : Controller
    {

        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public IngresoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Ingreso
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<Ingreso> registros;
            if (filter != null)
            {
                registros = await _context.Ingresos.Where(r => r.NombreIngreso.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Ingresos.ToListAsync();
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
            List<Ingreso>? data = null;
            if (data == null)
            {
                data = _context.Ingresos.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "Ingresos.xlsx";
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

        // GET: Ingreso/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingreso = await _context.Ingresos
                .FirstOrDefaultAsync(m => m.IdIngreso == id);
            if (ingreso == null)
            {
                return NotFound();
            }

            return View(ingreso);
        }

        // GET: Ingreso/Create
        public ActionResult Create()
        {
            ViewBag.TipoIngreso = Enum.GetValues(typeof(TipoIngreso));
            ViewBag.TipoPeriodo = Enum.GetValues(typeof(TipoPeriodo));
            
            return View();
        }

        // POST: Ingreso/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdIngreso,NombreIngreso,Tipo,Monto,Formula,Grabable,Periodo,FechaInicial,FechaFinal,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Ingreso ingreso)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(ingreso, true);
                _context.Add(ingreso);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            return View(ingreso);
        }

        // GET: Ingreso/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingreso = await _context.Ingresos.FindAsync(id);
            if (ingreso == null)
            {
                return NotFound();
            }
            ViewBag.TipoIngreso = Enum.GetValues(typeof(TipoIngreso));
            ViewBag.TipoPeriodo = Enum.GetValues(typeof(TipoPeriodo));
            return View(ingreso);
        }


        // POST: Ingreso/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdIngreso,NombreIngreso,Tipo,Monto,Formula,Grabable,Periodo,FechaInicial,FechaFinal,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Ingreso ingreso)
        {
            if (id != ingreso.IdIngreso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(ingreso, false);
                    _context.Update(ingreso);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngresoExists(ingreso.IdIngreso))
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
            return View(ingreso);
        }

        // GET: Ingreso/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingreso = await _context.Ingresos
                .FirstOrDefaultAsync(m => m.IdIngreso == id);
            if (ingreso == null)
            {
                return NotFound();
            }

            return View(ingreso);
        }

        // POST: Ingreso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingreso = await _context.Ingresos.FindAsync(id);
            try
            {

                if (ingreso != null)
                {
                    _context.Ingresos.Remove(ingreso);
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
                    TempData["Error"] = "Error: No puede elimiar el registro actual ya que se encuentra relacionado a otro Registro.";
                }
                else
                {
                    var message = ex.InnerException;
                    TempData["Error"] = "Error: " + message;
                }
                return View(ingreso);
            }

        }

        private bool IngresoExists(int id)
        {
            return _context.Ingresos.Any(e => e.IdIngreso == id);
        }

        private void SetCamposAuditoria(Ingreso record, bool bNewRecord)
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
            }
        }


    }
}
