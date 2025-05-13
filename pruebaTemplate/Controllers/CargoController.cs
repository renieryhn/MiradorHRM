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
    public class CargoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public CargoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cargo
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            ViewBag.Filter = filter;
            List<Cargo> registros;
            if (filter != null)
            {
                registros = await _context.Cargos.Where(r => r.NombreCargo.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Cargos.ToListAsync();
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

            var query = _context.Cargos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(c => EF.Functions.Like(c.NombreCargo, $"%{filter}%"));
            }

            var data = query
                        .Select(c => new
                {
                    c.IdCargo,
                    c.NombreCargo,
                    c.FuncionesCargo,
                    c.DescripcionCargo,
                    c.Activo,                
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

            string fileName = "Cargos.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Añadir la tabla al workbook con un nombre significativo para la hoja
                wb.Worksheets.Add(table, "Cargos");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }


        // GET: Cargo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos
                .FirstOrDefaultAsync(m => m.IdCargo == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // GET: Cargo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cargo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCargo,NombreCargo,DescripcionCargo,FuncionesCargo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(cargo, true);
                    _context.Add(cargo);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Cargo"))
                    {
                        TempData["error"] = "Error: El nombre del cargo ya está registrado. Por favor, ingrese un nombre diferente.";
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
            return View(cargo);
        }

        // GET: Cargo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo == null)
            {
                return NotFound();
            }
            return View(cargo);
        }

        // POST: Cargo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCargo,NombreCargo,DescripcionCargo,FuncionesCargo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Cargo cargo)
        {
            if (id != cargo.IdCargo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(cargo, false);
                    _context.Update(cargo);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CargoExists(cargo.IdCargo))
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
            return View(cargo);
        }

        // GET: Cargo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos
                .FirstOrDefaultAsync(m => m.IdCargo == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // POST: Cargo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var cargo = await _context.Cargos.FindAsync(id);
            try
            {
               
                if (cargo != null)
                {
                    _context.Cargos.Remove(cargo);
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
                return View(cargo);
            }

        }

        private bool CargoExists(int id)
        {
            return _context.Cargos.Any(e => e.IdCargo == id);
        }
        
        private void SetCamposAuditoria(Cargo record, bool bNewRecord)
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
