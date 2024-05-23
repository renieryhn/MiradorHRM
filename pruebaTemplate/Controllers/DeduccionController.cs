using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using System.Data;
using static PlanillaPM.cGeneralFun;
using static PlanillaPM.Models.Deduccion;
using static PlanillaPM.Models.Ingreso;

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

        // GET: DeduccionController
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<Deduccion> registros;
            if (filter != null)
            {
                registros = await _context.Deduccions.Where(r => r.NombreDeduccion.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Deduccions.ToListAsync();
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
            List<Deduccion>? data = null;
            if (data == null)
            {
                data = _context.Deduccions.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "Deducciones.xlsx";
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

        // GET: DeduccionController/Details/5
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

        // GET: DeduccionController/Create
        public ActionResult Create()
        {
            ViewBag.TipoDeduccion = Enum.GetValues(typeof(TipoDeduccion));
            return View();
        }

        // POST: DeduccionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDeduccion,NombreDeduccion,Tipo,Monto,Formula,DeducibleImpuesto,BasadoEnTodo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Deduccion deduccion)
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
                TempData["Error"] = "Error: " + message;
            }
            return View(deduccion);
        }

        // GET: DeduccionController/Edit/5
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
            ViewBag.TipoDeduccion = Enum.GetValues(typeof(TipoDeduccion));
           
            return View(deduccion);
        }

        // POST: DeduccionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDeduccion,NombreDeduccion,Tipo,Monto,Formula,DeducibleImpuesto,BasadoEnTodo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Deduccion deduccion)
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
            return View(deduccion);
        }

        // GET: Ingreso/Delete/5
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


        // POST: DeduccionController/Delete/5
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
                    TempData["Error"] = "Error: No puede elimiar el registro actual ya que se encuentra relacionado a otro Registro.";
                }
                else
                {
                    var message = ex.InnerException;
                    TempData["Error"] = "Error: " + message;
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
