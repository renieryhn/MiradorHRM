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
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEdit([FromBody] ImpuestoTabla impuestoTabla)
        {
            
            if (ModelState.IsValid)
            {

                // Adjust validation for the Porcentaje field
                if (impuestoTabla.Porcentaje.HasValue && (impuestoTabla.Porcentaje < -999.99m || impuestoTabla.Porcentaje > 9999.99m))
                {
                    ModelState.AddModelError("Porcentaje", "El porcentaje debe estar entre -999.99 y 9999.99.");
                }
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["Error"] = "Error: " + message;
                    return Json(new { success = false, error = message });
                }

                if (impuestoTabla.IdImpuestoTabla > 0)
                {
                    SetCamposAuditoria(impuestoTabla, false);
                    _context.Update(impuestoTabla);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido Actualizado exitosamente.";
                }
                else
                {
                    SetCamposAuditoria(impuestoTabla, true);
                    _context.Add(impuestoTabla);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                }


              
               return Json(new { success = true, message = TempData["success"] });
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
                return Json(new { success = false, error = message });
            }
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromBody] ImpuestoTabla impuestoTabla)
        {
            try
            {
                var id = impuestoTabla.IdImpuestoTabla;
                var impuesto = await _context.ImpuestoTablas.FindAsync(id);

                if (impuesto == null)
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el impuesto. El registro no se encontró.";
                    return Json(new { success = false, error = "Registro no encontrado." });
                }

                _context.ImpuestoTablas.Remove(impuesto);
                await _context.SaveChangesAsync();

                TempData["Success"] = "El registro ha sido eliminado exitosamente.";
                return Json(new { success = true, message = "El registro ha sido eliminado exitosamente." });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_"))
                {
                    TempData["Error"] = "Error: No se puede eliminar el registro actual porque está relacionado con otro registro.";
                    return Json(new { success = false, error = "No se puede eliminar el registro porque está relacionado con otro registro." });
                }
                else
                {
                    TempData["Error"] = "Error: " + ex.Message;
                    return Json(new { success = false, error = ex.Message });
                }
            }
        }



        

        private bool ImpuestoTablaExists(int id)
        {
            return _context.ImpuestoTablas.Any(e => e.IdImpuestoTabla == id);
        }
        

        private void SetCamposAuditoria(ImpuestoTabla record, bool bNewRecord)
        {
            if (record == null) return;

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
