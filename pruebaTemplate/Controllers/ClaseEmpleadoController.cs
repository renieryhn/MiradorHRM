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
    public class ClaseEmpleadoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ClaseEmpleadoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ClaseEmpleado
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            ViewBag.Filter = filter;
            List<ClaseEmpleado> registros;
            if (filter != null)
            {
                registros = await _context.ClaseEmpleados.Where(r => r.NombreClaseEmpleado.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.ClaseEmpleados.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.ClaseEmpleados.Include(c => c.IdHorarioNavigation);
            var IdHorarioNavigation = await _context.Horarios.ToListAsync();
            return View(data);
        }
        public ActionResult Download(string? filter)
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();

            var query = _context.ClaseEmpleados.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(c => EF.Functions.Like(c.NombreClaseEmpleado, $"%{filter}%"));
            }

            var data = query
                        .Select(c => new
                {
                    c.IdClaseEmpleado,
                    c.NombreClaseEmpleado,
                    Horario = c.IdHorarioNavigation != null ? c.IdHorarioNavigation.NombreHorario : "Sin Horario", 
                    c.Activo
                   
                   
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

            string fileName = "ClaseEmpleados.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Añadir la tabla al workbook con un nombre significativo para la hoja
                wb.Worksheets.Add(table, "ClaseEmpleados");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // GET: ClaseEmpleado/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claseEmpleado = await _context.ClaseEmpleados
                .Include(c => c.IdHorarioNavigation)
                .FirstOrDefaultAsync(m => m.IdClaseEmpleado == id);
            if (claseEmpleado == null)
            {
                return NotFound();
            }

            return View(claseEmpleado);
        }

        // GET: ClaseEmpleado/Create
        public IActionResult Create()
        {
            ViewData["IdHorario"] = new SelectList(_context.Horarios, "IdHorario", "NombreHorario");
            return View();
        }

        // POST: ClaseEmpleado/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdClaseEmpleado,NombreClaseEmpleado,IdHorario,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] ClaseEmpleado claseEmpleado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(claseEmpleado, true);
                    _context.Add(claseEmpleado);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_ClaseEmpleado"))
                    {
                        TempData["error"] = "Error: El nombre de la clase de empleado ya está registrado. Por favor, ingrese un nombre diferente.";
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
            ViewData["IdHorario"] = new SelectList(_context.Horarios, "IdHorario", "NombreHorario", claseEmpleado.IdHorario);
            return View(claseEmpleado);
        }

        // GET: ClaseEmpleado/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claseEmpleado = await _context.ClaseEmpleados.FindAsync(id);
            if (claseEmpleado == null)
            {
                return NotFound();
            }
            ViewData["IdHorario"] = new SelectList(_context.Horarios, "IdHorario", "NombreHorario", claseEmpleado.IdHorario);
            return View(claseEmpleado);
        }

        // POST: ClaseEmpleado/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdClaseEmpleado,NombreClaseEmpleado,IdHorario,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] ClaseEmpleado claseEmpleado)
        {
            if (id != claseEmpleado.IdClaseEmpleado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(claseEmpleado, false);
                    _context.Update(claseEmpleado);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaseEmpleadoExists(claseEmpleado.IdClaseEmpleado))
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
            ViewData["IdHorario"] = new SelectList(_context.Horarios, "IdHorario", "NombreHorario", claseEmpleado.IdHorario);
            return View(claseEmpleado);
        }

        // GET: ClaseEmpleado/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claseEmpleado = await _context.ClaseEmpleados
                .Include(c => c.IdHorarioNavigation)
                .FirstOrDefaultAsync(m => m.IdClaseEmpleado == id);
            if (claseEmpleado == null)
            {
                return NotFound();
            }

            return View(claseEmpleado);
        }

        // POST: ClaseEmpleado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var claseEmpleado = await _context.ClaseEmpleados.FindAsync(id);
            try
            {
               
                if (claseEmpleado != null)
                {
                    _context.ClaseEmpleados.Remove(claseEmpleado);
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
                return View(claseEmpleado);
            }

        }

        private bool ClaseEmpleadoExists(int id)
        {
            return _context.ClaseEmpleados.Any(e => e.IdClaseEmpleado == id);
        }
        
        private void SetCamposAuditoria(ClaseEmpleado record, bool bNewRecord)
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
