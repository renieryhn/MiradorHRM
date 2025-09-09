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
    public class EmpleadoHorarioController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoHorarioController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoHorario
        public async Task<IActionResult> Index(int pg, string? filter = null, string? idEmpleado = null, int? estado = null)
        {
            var query = _context.EmpleadoHorarios
                                .Include(e => e.IdEmpleadoNavigation)
                                .Include(e => e.IdHorarioBaseNavigation)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdEmpleadoNavigation.NombreCompleto.ToLower().Contains(filter.ToLower()));
            }

            if (!string.IsNullOrEmpty(idEmpleado))
            {
                if (int.TryParse(idEmpleado, out int empleadoId))
                {
                    query = query.Where(r => r.IdEmpleado == empleadoId);
                }
            }

            if (estado.HasValue)
            {
                bool estadoBool = estado.Value == 0; // 0 = Activo, 1 = Inactivo
                query = query.Where(r => r.Activo == estadoBool);
            }

            // ViewBags para filtros
            ViewBag.Filter = filter;
            ViewBag.CurrentIdEmpleado = idEmpleado;
            ViewBag.CurrentEstado = estado;

            List<EmpleadoHorario> registros;
            registros = await query.Include(e => e.IdEmpleadoNavigation).ToListAsync();

            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            // Combo de empleados
            var empleados = await _context.Empleados.ToListAsync();
            ViewData["IdEmpleado"] = new SelectList(empleados, "IdEmpleado", "NombreCompleto", idEmpleado);

            return View(data);
        }
        public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<EmpleadoHorario>? data = null;
             if (data == null)
             {
                data = _context.EmpleadoHorarios.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "EmpleadoHorarios.xlsx";
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
        // GET: EmpleadoHorario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHorario = await _context.EmpleadoHorarios
         .Include(e => e.IdEmpleadoNavigation)
         .Include(e => e.IdHorarioBaseNavigation)
         .FirstOrDefaultAsync(m => m.IdEmpleadoHorario == id);
            if (empleadoHorario == null)
            {
                return NotFound();
            }

            return View(empleadoHorario);
        }

        // GET: EmpleadoHorario/Create
        public IActionResult Create()
        {
          
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            ViewData["IdHorario"] = new SelectList(_context.Horarios, "IdHorario", "NombreHorario");
            return View();
        }

        // POST: EmpleadoHorario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoHorario,IdEmpleado,IdHorarioBase,IndSabado,IndDomingo,LunDesde,LunHasta,MarDesde,MarHasta,MieDesde,MieHasta,JueDesde,JueHasta,VieDesde,VieHasta,SabDesde,SabHasta,DomDesde,DomHasta,TotalHorasSemana,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor,LunRecesoDesde,LunRecesoHasta,MarRecesoDesde,MarRecesoHasta,MieRecesoDesde,MieRecesoHasta,JueRecesoDesde,JueRecesoHasta,VieRecesoDesde,VieRecesoHasta,SabRecesoDesde,SabRecesoHasta,DomRecesoDesde,DomRecesoHasta,HorasLunes,HorasMartes,HorasMiercoles,HorasJueves,HorasViernes,HorasSabado,HorasDomingo")] EmpleadoHorario empleadoHorario)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var horarioExiste = await _context.Horarios.AnyAsync(h => h.IdHorario == empleadoHorario.IdHorarioBase && h.Activo);
                    if (!horarioExiste)
                    {
                        TempData["error"] = "Error: El horario seleccionado no existe o está inactivo.";
                        return View(empleadoHorario);
                    }



                    SetCamposAuditoria(empleadoHorario, true);
                    _context.Add(empleadoHorario);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["error"] = "Error de validación: " + message;
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["error"] = "Error al guardar en base de datos: " + ex.InnerException?.Message;
            }

            return View(empleadoHorario);
        }

        // GET: EmpleadoHorario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHorario = await _context.EmpleadoHorarios.FindAsync(id);
            if (empleadoHorario == null)
            {
                return NotFound();
            }

            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoHorario.IdEmpleado);
            ViewData["IdHorarioBase"] = new SelectList(_context.Horarios.Where(h => h.Activo), "IdHorario", "NombreHorario", empleadoHorario.IdHorarioBase);

            return View(empleadoHorario);
        }

        // POST: EmpleadoHorario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoHorario,IdEmpleado,IdHorarioBase,IndSabado,IndDomingo,LunDesde,LunHasta,MarDesde,MarHasta,MieDesde,MieHasta,JueDesde,JueHasta,VieDesde,VieHasta,SabDesde,SabHasta,DomDesde,DomHasta,TotalHorasSemana,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor,LunRecesoDesde,LunRecesoHasta,MarRecesoDesde,MarRecesoHasta,MieRecesoDesde,MieRecesoHasta,JueRecesoDesde,JueRecesoHasta,VieRecesoDesde,VieRecesoHasta,SabRecesoDesde,SabRecesoHasta,DomRecesoDesde,DomRecesoHasta,HorasLunes,HorasMartes,HorasMiercoles,HorasJueves,HorasViernes,HorasSabado,HorasDomingo")] EmpleadoHorario empleadoHorario)
        {
            if (id != empleadoHorario.IdEmpleadoHorario)
            {
                return NotFound();
            }

            if (empleadoHorario.IndSabado &&
            (empleadoHorario.SabDesde == TimeSpan.Zero || empleadoHorario.SabHasta == TimeSpan.Zero))
            {
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoHorario.IdEmpleado);
                ViewData["IdHorarioBase"] = new SelectList(_context.Horarios.Where(h => h.Activo), "IdHorario", "NombreHorario", empleadoHorario.IdHorarioBase);
                TempData["error"] = "El horario del sábado no puede tener valores en 00:00 si está activo.";
                return View(empleadoHorario);
            }

            if (empleadoHorario.IndDomingo &&
                (empleadoHorario.DomDesde == TimeSpan.Zero || empleadoHorario.DomHasta == TimeSpan.Zero))
            {
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoHorario.IdEmpleado);
                ViewData["IdHorarioBase"] = new SelectList(_context.Horarios.Where(h => h.Activo), "IdHorario", "NombreHorario", empleadoHorario.IdHorarioBase);
                TempData["success"] = "El horario del domingo no puede tener valores en 00:00 si está activo.";
                return View(empleadoHorario);
            }


            if (ModelState.IsValid)
            {
                try
                {
                    var original = await _context.EmpleadoHorarios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(e => e.IdEmpleadoHorario == id);

                    if (original == null)
                        return NotFound();

                    // Conservar campos que no vienen del formulario
                    empleadoHorario.FechaCreacion = original.FechaCreacion;
                    empleadoHorario.CreadoPor = original.CreadoPor;

                    SetCamposAuditoria(empleadoHorario, false);
                    _context.Update(empleadoHorario);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "El registro ha sido actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoHorarioExists(empleadoHorario.IdEmpleadoHorario))
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
            return View(empleadoHorario);
        }

        // GET: EmpleadoHorario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHorario = await _context.EmpleadoHorarios
                 .Include(e => e.IdEmpleadoNavigation)
         .Include(e => e.IdHorarioBaseNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoHorario == id);
            if (empleadoHorario == null)
            {
                return NotFound();
            }

            return View(empleadoHorario);
        }

        // POST: EmpleadoHorario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var empleadoHorario = await _context.EmpleadoHorarios.FindAsync(id);
            try
            {
               
                if (empleadoHorario != null)
                {
                    _context.EmpleadoHorarios.Remove(empleadoHorario);
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
                return View(empleadoHorario);
            }

        }

        private bool EmpleadoHorarioExists(int id)
        {
            return _context.EmpleadoHorarios.Any(e => e.IdEmpleadoHorario == id);
        }
        
        private void SetCamposAuditoria(EmpleadoHorario record, bool bNewRecord)
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



        [HttpGet]
        public async Task<IActionResult> ObtenerHorarioPorEmpleado(int idEmpleado)
        {
            var horarioId = await _context.Empleados
                .Where(e => e.IdEmpleado == idEmpleado && e.Activo)
                .Where(e => e.IdClaseEmpleadoNavigation != null && e.IdClaseEmpleadoNavigation.Activo)
                .Select(e => e.IdClaseEmpleadoNavigation.IdHorario)
                .FirstOrDefaultAsync();

            if (horarioId == null)
                return NotFound();

            return Json(new { idHorario = horarioId });
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerHorarioBase(int id)
        {
            var horario = await _context.Horarios
               .Where(h => h.IdHorario == id && h.Activo)
                .Select(h => new {
                    h.IndLunes,
                    h.LunDesde,
                    h.LunHasta,
                    LunRecesoDesde = h.ComidaDesde,
                    LunRecesoHasta = h.ComidaHasta,

                    h.IndMartes,
                    h.MarDesde,
                    h.MarHasta,
                    MarRecesoDesde = h.ComidaDesde,
                    MarRecesoHasta = h.ComidaHasta,

                    h.IndMiercoles,
                    h.MieDesde,
                    h.MieHasta,
                    MieRecesoDesde = h.ComidaDesde,
                    MieRecesoHasta = h.ComidaHasta,

                    h.IndJueves,
                    h.JueDesde,
                    h.JueHasta,
                    JueRecesoDesde = h.ComidaDesde,
                    JueRecesoHasta = h.ComidaHasta,

                    h.IndViernes,
                    h.VieDesde,
                    h.VieHasta,
                    VieRecesoDesde = h.ComidaDesde,
                    VieRecesoHasta = h.ComidaHasta,

                    h.IndSabado,
                    h.SabDesde,
                    h.SabHasta,
                    SabRecesoDesde = h.ComidaDesde,
                    SabRecesoHasta = h.ComidaHasta,

                    h.IndDomingo,
                    h.DomDesde,
                    h.DomHasta,
                    DomRecesoDesde = h.ComidaDesde,
                    DomRecesoHasta = h.ComidaHasta
                })
                .FirstOrDefaultAsync();

            if (horario == null)
                return NotFound();

            return Json(horario);
        }


    }
}
