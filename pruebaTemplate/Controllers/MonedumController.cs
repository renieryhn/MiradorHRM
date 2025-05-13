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

using PlanillaPM.Models;

namespace PlanillaPM.Controllers
{
    public class MonedumController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public MonedumController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Monedum
        public async Task<IActionResult> Index(int pg, string? filter)
        {

            ViewBag.Filter = filter;
            List<Monedum> registros;
            if (filter != null)
            {
                registros = await _context.Moneda.Where(r => r.NombreMoneda.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Moneda.ToListAsync();
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

        [HttpGet]
        public ActionResult Download(string? filter)
        {
            var monedasQuery = _context.Moneda.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                monedasQuery = monedasQuery.Where(m => m.NombreMoneda.ToLower().Contains(filter.ToLower()));
            }

            var data = monedasQuery
                .Select(m => new
                {
                    m.IdMoneda,
                    NombreMoneda = m.NombreMoneda,
                    Simbolo = m.Simbolo,
                    Activo = m.Activo ? "Sí" : "No"
                })
                .ToList();

            // Verificar si la lista está vacía
            if (!data.Any())
            {

                TempData["error"] = "No se encontraron Registros.";
                return RedirectToAction(nameof(Index));
            }

            // Convertir la lista en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = "Moneda.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Monedas");
                worksheet.Columns().AdjustToContents(); // Ajustar el ancho de las columnas automáticamente

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }


        // GET: Monedum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monedum = await _context.Moneda
                .FirstOrDefaultAsync(m => m.IdMoneda == id);
            if (monedum == null)
            {
                return NotFound();
            }

            return View(monedum);
        }

        // GET: Monedum/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Monedum/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMoneda,NombreMoneda,Simbolo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Monedum monedum)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(monedum, true);
                    _context.Add(monedum);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Moneda"))
                    {
                        TempData["error"] = "Error: El nombre de la moneda ya está registrado. Por favor, ingrese un nombre diferente.";
                    }
                    else
                    {
                        TempData["error"] = "Error: Ocurrió un error al intentar crear el registro. Por favor, inténtelo de nuevo.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Error: Ocurrió un error inesperado. Por favor, inténtelo de nuevo.";
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            return View(monedum);
        }

        // GET: Monedum/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monedum = await _context.Moneda.FindAsync(id);
            if (monedum == null)
            {
                return NotFound();
            }
            return View(monedum);
        }

        // POST: Monedum/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMoneda,NombreMoneda,Simbolo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Monedum monedum)
        {
            if (id != monedum.IdMoneda)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(monedum, false);
                    _context.Update(monedum);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonedumExists(monedum.IdMoneda))
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
            return View(monedum);
        }

        // GET: Monedum/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monedum = await _context.Moneda
                .FirstOrDefaultAsync(m => m.IdMoneda == id);
            if (monedum == null)
            {
                return NotFound();
            }

            return View(monedum);
        }

        // POST: Monedum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var monedum = await _context.Moneda.FindAsync(id);
            try
            {
               
                if (monedum != null)
                {
                    _context.Moneda.Remove(monedum);
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
                return View(monedum);
            }

        }

        private bool MonedumExists(int id)
        {
            return _context.Moneda.Any(e => e.IdMoneda == id);
        }

        private void SetCamposAuditoria(Monedum record, bool bNewRecord)
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
