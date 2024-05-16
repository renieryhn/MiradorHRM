using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using System.Data;
using static PlanillaPM.cGeneralFun;
using static PlanillaPM.Models.EmpleadoContrato;
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


        // GET: ImpuestosController
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<Impuesto> registros;
            if (filter != null)
            {
                registros = await _context.Impuestos.Where(r => r.NombreImpuesto.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Impuestos.ToListAsync();
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

        // GET: ImpuestosController/Details/5
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

        // GET: ImpuestosController/Create
        public ActionResult Create()
        {
            ViewBag.TipoImpuesto = Enum.GetValues(typeof(TipoImpuesto));

            return View();
        }

        // POST: ImpuestosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdImpuesto,NombreImpuesto,Tipo,Monto,Formula,Grabable,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Impuesto impuesto)
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
                TempData["Error"] = "Error: " + message;
            }
            return View(impuesto);
        }

        [HttpGet]
        public IActionResult MostrarModalImpuestoTabla(int impuestoId)
        {
            ViewBag.ImpuestoId = impuestoId;
            var impuestoTabla = _context.ImpuestoTablas.FirstOrDefault(it => it.IdImpuesto == impuestoId);
            if (impuestoTabla == null)
            {
                // Si no se encuentra ImpuestoTabla asociado, puedes crear uno nuevo o manejarlo según sea necesario
                impuestoTabla = new ImpuestoTabla();
            }

            var viewModel = new ImpuestoTablaListaYModelo
            {
                ImpuestoTabla = impuestoTabla,
                //ListaImpuestoTabla = _context.ImpuestoTablas.ToList()
                 ListaImpuestoTabla = _context.ImpuestoTablas.Where(it => it.IdImpuesto == impuestoId).ToList()
            };


            if (impuestoTabla.IdImpuestoTabla == 0)
            {
                return PartialView("_CrearImpuestoTabla", viewModel);
            }
            else
            {
                return PartialView("_EditarImpuestoTabla", viewModel);
            }
        }


        // GET: ImpuestosController/Edit/5
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
            ViewBag.TipoImpuesto = Enum.GetValues(typeof(TipoImpuesto));

            return View(impuesto);
           
        }

        // POST: ImpuestosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdImpuesto,NombreImpuesto,Tipo,Monto,Formula,Grabable,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Impuesto impuesto)
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

        // GET: ImpuestosController/Delete/5
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

        // POST: ImpuestosController/Delete/5
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
