using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using System.Data;
using static PlanillaPM.cGeneralFun;

namespace PlanillaPM.Controllers
{
    public class ImpuestoTablaController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ImpuestoTablaController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: ImpuestosController/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: ImpuestosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdImpuestoTabla,IdImpuesto,Desde,Hasta,Monto,Porcentaje,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] ImpuestoTabla impuestoTabla)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(impuestoTabla, true);
                _context.Add(impuestoTabla);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";

                return Redirect(Request.Headers["Referer"].ToString());

            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            return View(impuestoTabla);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var impuestoTabla = await _context.ImpuestoTablas.FindAsync(id);
            if (impuestoTabla == null)
            {
                return NotFound();
            }

            var viewModel = new ImpuestoTablaListaYModelo
            {
                ImpuestoTabla = impuestoTabla,
                ListaImpuestoTabla = await _context.ImpuestoTablas.ToListAsync()
            };

            return View(viewModel);
        }


        // POST: ImpuestosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdImpuestoTabla,IdImpuesto,Desde,Hasta,Monto,Porcentaje,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] ImpuestoTablaListaYModelo impuestoTablaListaYModelo)
        {         

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(impuestoTablaListaYModelo.ImpuestoTabla, false);
                    _context.Update(impuestoTablaListaYModelo.ImpuestoTabla);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";

                    return Redirect(Request.Headers["Referer"].ToString());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImpuestoTablaExists(impuestoTablaListaYModelo.ImpuestoTabla.IdImpuestoTabla))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // GET: ImpuestosController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var impuestoTabla = await _context.ImpuestoTablas
                .FirstOrDefaultAsync(m => m.IdImpuestoTabla == id);
            if (impuestoTabla == null)
            {
                return NotFound();
            }

            return View(impuestoTabla);
        }

        // POST: ImpuestosController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var impuestoTabla = await _context.ImpuestoTablas.FindAsync(id);
            try
            {

                if (impuestoTabla != null)
                {
                    _context.ImpuestoTablas.Remove(impuestoTabla);
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
                return View(impuestoTabla);
            }

        }

        private bool ImpuestoTablaExists(int id)
        {
            return _context.ImpuestoTablas.Any(e => e.IdImpuestoTabla == id);
        }
        

        private void SetCamposAuditoria(ImpuestoTabla record, bool bNewRecord)
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
                if (record.FechaCreacion.ToString() == "1/1/0001 00:00:00")
                {
                    record.FechaCreacion = now;
                }
                if (record.CreadoPor == null)
                {
                    record.CreadoPor = CurrentUser;
                }
            }
        }
    }
}
