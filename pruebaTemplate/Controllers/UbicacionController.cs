using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using System.Data;
using static PlanillaPM.cGeneralFun;
using static PlanillaPM.Models.Ubicacion;

namespace PlanillaPM.Controllers
{
    public class UbicacionController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public UbicacionController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: ImpuestosController
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<Ubicacion> registros;
            if (filter != null)
            {
                registros = await _context.Ubicaciones.Where(r => r.NombreUbicacion.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Ubicaciones.ToListAsync();
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
            List<Ubicacion>? data = null;
            if (data == null)
            {
                data = _context.Ubicaciones.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "Ubicaciones.xlsx";
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

            var ubicacion = await _context.Ubicaciones
                .FirstOrDefaultAsync(m => m.IdUbicacion == id);
            if (ubicacion == null)
            {
                return NotFound();
            }

            return View(ubicacion);
        }

        // GET: ImpuestosController/Create
        public ActionResult Create()
        {
          
            return View();
        }

        // POST: ImpuestosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUbicacion,NombreUbicacion,Ciudad,Direccion,Telefono,Email,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Ubicacion ubicacion)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(ubicacion, true);
                _context.Add(ubicacion);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            return View(ubicacion);
        }

        // GET: ImpuestosController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ubicacion = await _context.Ubicaciones.FindAsync(id);
            if (ubicacion == null)
            {
                return NotFound();
            }
           
            return View(ubicacion);

        }

        // POST: ImpuestosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUbicacion,NombreUbicacion,Ciudad,Direccion,Telefono,Email,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Ubicacion ubicacion)
        {
            if (id != ubicacion.IdUbicacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(ubicacion, false);
                    _context.Update(ubicacion);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UbicacionExists(ubicacion.IdUbicacion))
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
            return View(ubicacion);
        }

        // GET: ImpuestosController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ubicacion = await _context.Ubicaciones
                .FirstOrDefaultAsync(m => m.IdUbicacion == id);
            if (ubicacion == null)
            {
                return NotFound();
            }

            return View(ubicacion);
        }

        // POST: ImpuestosController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ubicacion = await _context.Ubicaciones.FindAsync(id);
            try
            {

                if (ubicacion != null)
                {
                    _context.Ubicaciones.Remove(ubicacion);
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
                return View(ubicacion);
            }

        }

        private bool UbicacionExists(int id)
        {
            return _context.Ubicaciones.Any(e => e.IdUbicacion == id);
        }
        private void SetCamposAuditoria(Ubicacion record, bool bNewRecord)
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
