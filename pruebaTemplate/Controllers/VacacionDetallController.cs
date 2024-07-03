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
using static PlanillaPM.Models.VacacionDetalle;

namespace PlanillaPM.Controllers
{
    public class VacacionDetallController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public VacacionDetallController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: VacacionDetall
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<VacacionDetalle> registros;
            if (filter != null)
            {
                registros = await _context.VacacionDetalles.Where(r => r.IdVacacionNavigation.ModificadoPor.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.VacacionDetalles.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.VacacionDetalles.Include(v => v.IdEmpleadoNavigation).Include(v => v.IdVacacionNavigation);
            return View(data);
        }
         public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<VacacionDetalle>? data = null;
             if (data == null)
             {
                data = _context.VacacionDetalles.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "VacacionDetalles.xlsx";
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
        // GET: VacacionDetall/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacionDetalle = await _context.VacacionDetalles
                .Include(v => v.IdEmpleadoNavigation)
                .Include(v => v.IdVacacionNavigation)
                .FirstOrDefaultAsync(m => m.IdVacacionDetalle == id);
            if (vacacionDetalle == null)
            {
                return NotFound();
            }

            return View(vacacionDetalle);
        }

        // GET: VacacionDetall/Create
        public IActionResult Create()
        {
            ViewBag.Estado = Enum.GetValues(typeof(Estado));
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            ViewData["IdVacacion"] = new SelectList(_context.Vacacions, "IdVacacion", "IdVacacion");
            return View();
        }

        // POST: VacacionDetall/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVacacionDetalle,IdVacacion,IdEmpleado,FechaSolicitud,FechaInicio,FechaFin,NumeroDiasSolicitados,EstadoSolicitud,AprobadoPor,DiasAprobados,ComentariosAprobador,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] VacacionDetalle vacacionDetalle)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(vacacionDetalle, true);
                _context.Add(vacacionDetalle);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacionDetalle.IdEmpleado);
            ViewData["IdVacacion"] = new SelectList(_context.Vacacions, "IdVacacion", "IdVacacion", vacacionDetalle.IdVacacion);
            return View(vacacionDetalle);
        }

        // GET: VacacionDetall/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacionDetalle = await _context.VacacionDetalles.FindAsync(id);
            if (vacacionDetalle == null)
            {
                return NotFound();
            }
            ViewBag.Estado = Enum.GetValues(typeof(VacacionDetalle.Estado));
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacionDetalle.IdEmpleado);
            ViewData["IdVacacion"] = new SelectList(_context.Vacacions, "IdVacacion", "IdVacacion", vacacionDetalle.IdVacacion);
            return View(vacacionDetalle);
        }

        // POST: VacacionDetall/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVacacionDetalle,IdVacacion,IdEmpleado,FechaSolicitud,FechaInicio,FechaFin,NumeroDiasSolicitados,EstadoSolicitud,AprobadoPor,DiasAprobados,ComentariosAprobador,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] VacacionDetalle vacacionDetalle)
        {
            if (id != vacacionDetalle.IdVacacionDetalle)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(vacacionDetalle, false);
                    _context.Update(vacacionDetalle);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacacionDetalleExists(vacacionDetalle.IdVacacionDetalle))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", vacacionDetalle.IdEmpleado);
            ViewData["IdVacacion"] = new SelectList(_context.Vacacions, "IdVacacion", "IdVacacion", vacacionDetalle.IdVacacion);
            return View(vacacionDetalle);
        }

        // GET: VacacionDetall/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacacionDetalle = await _context.VacacionDetalles
                .Include(v => v.IdEmpleadoNavigation)
                .Include(v => v.IdVacacionNavigation)
                .FirstOrDefaultAsync(m => m.IdVacacionDetalle == id);
            if (vacacionDetalle == null)
            {
                return NotFound();
            }

            return View(vacacionDetalle);
        }

        // POST: VacacionDetall/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var vacacionDetalle = await _context.VacacionDetalles.FindAsync(id);
            try
            {
               
                if (vacacionDetalle != null)
                {
                    _context.VacacionDetalles.Remove(vacacionDetalle);
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
                return View(vacacionDetalle);
            }

        }

        private bool VacacionDetalleExists(int id)
        {
            return _context.VacacionDetalles.Any(e => e.IdVacacionDetalle == id);
        }
        
        private void SetCamposAuditoria(VacacionDetalle record, bool bNewRecord)
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
