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
using PlanillaPM.Models;
using PlanillaPM.Servicio;
using Microsoft.AspNetCore.Identity;
using PlanillaPM.ViewModel;

namespace PlanillaPM.Controllers
{
    public class BancoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public BancoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Banco
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            ViewBag.Filter = filter;
            List<Banco> registros;
            if (filter != null)
            {
                registros = await _context.Bancos.Where(r => r.NombreBanco.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Bancos.ToListAsync();
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
            ListtoDataTableConverter converter = new ListtoDataTableConverter();

            // Filtrar si se proporciona un valor
            var bancosQuery = _context.Bancos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                bancosQuery = bancosQuery.Where(b => b.NombreBanco.ToLower().Contains(filter.ToLower()));
            }

            // Obtener los datos filtrados
            var data = bancosQuery
                .Select(b => new
                {
                    b.IdBanco,
                    b.NombreBanco,
                    Activo = b.Activo ? "Sí" : "No"
                })
                .ToList();

            // Verificar si la lista está vacía
            if (!data.Any())
            {
                TempData["error"] = "No se encontraron Registros.";
                return RedirectToAction(nameof(Index));
            }

            // Convertir la lista a DataTable
            DataTable table = converter.ToDataTable(data);

            string fileName = "Bancos.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Añadir la tabla al workbook con un nombre significativo para la hoja
                wb.Worksheets.Add(table, "Bancos");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }


        // GET: Banco/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banco = await _context.Bancos
                .FirstOrDefaultAsync(m => m.IdBanco == id);
            if (banco == null)
            {
                return NotFound();
            }

            return View(banco);
        }

        // GET: Banco/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Banco/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdBanco,NombreBanco,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Banco banco)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(banco, true);
                    _context.Add(banco);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Banco"))
                    {
                        TempData["error"] = "Error: El nombre del banco ya está registrado. Por favor, ingrese un nombre diferente.";
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
            return View(banco);
        }

        // GET: Banco/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banco = await _context.Bancos.FindAsync(id);
            if (banco == null)
            {
                return NotFound();
            }
            return View(banco);
        }

        // POST: Banco/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdBanco,NombreBanco,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Banco banco)
        {
            if (id != banco.IdBanco)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(banco, false);
                    _context.Update(banco);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BancoExists(banco.IdBanco))
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
            return View(banco);
        }

        // GET: Banco/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banco = await _context.Bancos
                .FirstOrDefaultAsync(m => m.IdBanco == id);
            if (banco == null)
            {
                return NotFound();
            }

            return View(banco);
        }

        // POST: Banco/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banco = await _context.Bancos.FindAsync(id);
            try
            {

                if (banco != null)
                {
                    _context.Bancos.Remove(banco);
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
                return View(banco);
            }

        }

        private bool BancoExists(int id)
        {
            return _context.Bancos.Any(e => e.IdBanco == id);
        }

        private void SetCamposAuditoria(Banco record, bool bNewRecord)
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
