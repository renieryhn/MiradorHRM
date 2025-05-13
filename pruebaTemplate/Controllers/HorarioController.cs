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
    public class HorarioController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public HorarioController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Horario
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            ViewBag.Filter = filter;
            List<Horario> registros;
            if (filter != null)
            {
                registros = await _context.Horarios.Where(r => r.NombreHorario.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Horarios.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.Horarios.Include(h => h.IdTipoHorarioNavigation);
            return View(data);
        }

        [HttpGet]
        public ActionResult Download(string? filter)
        {
            var horariosQuery = _context.Horarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                horariosQuery = horariosQuery
                    .Where(h => h.NombreHorario.ToLower().Contains(filter.ToLower()));
            }

            var data = horariosQuery
                                .Select(h => new
                        {
                            h.IdHorario,
                            Horario = h.NombreHorario,
                            TipoHorario = h.IdTipoHorarioNavigation.NombreTipoHorario, // Asumiendo que TipoHorario tiene una propiedad NombreTipoHorario
                            Turno = h.TurnoNumero,
                            TrabajaLunes = h.IndLunes ? "Sí" : "No",
                            TrabajaMartes = h.IndMartes ? "Sí" : "No",
                            TrabajaMiércoles = h.IndMiercoles ? "Sí" : "No",
                            TrabajaJueves = h.IndJueves ? "Sí" : "No",
                            TrabajaViernes = h.IndViernes ? "Sí" : "No",
                            TrabajaSábado = h.IndSabado ? "Sí" : "No",
                            TrabajaDomingo = h.IndDomingo ? "Sí" : "No",
                            LunDesde = h.LunDesde.HasValue ? h.LunDesde.Value.ToString("hh\\:mm") : "",
                            LunHasta = h.LunHasta.HasValue ? h.LunHasta.Value.ToString("hh\\:mm") : "",
                            MarDesde = h.MarDesde.HasValue ? h.MarDesde.Value.ToString("hh\\:mm") : "",
                            MarHasta = h.MarHasta.HasValue ? h.MarHasta.Value.ToString("hh\\:mm") : "",
                            MieDesde = h.MieDesde.HasValue ? h.MieDesde.Value.ToString("hh\\:mm") : "",
                            MieHasta = h.MieHasta.HasValue ? h.MieHasta.Value.ToString("hh\\:mm") : "",
                            JueDesde = h.JueDesde.HasValue ? h.JueDesde.Value.ToString("hh\\:mm") : "",
                            JueHasta = h.JueHasta.HasValue ? h.JueHasta.Value.ToString("hh\\:mm") : "",
                            VieDesde = h.VieDesde.HasValue ? h.VieDesde.Value.ToString("hh\\:mm") : "",
                            VieHasta = h.VieHasta.HasValue ? h.VieHasta.Value.ToString("hh\\:mm") : "",
                            SabDesde = h.SabDesde.HasValue ? h.SabDesde.Value.ToString("hh\\:mm") : "",
                            SabHasta = h.SabHasta.HasValue ? h.SabHasta.Value.ToString("hh\\:mm") : "",
                            DomDesde = h.DomDesde.HasValue ? h.DomDesde.Value.ToString("hh\\:mm") : "",
                            DomHasta = h.DomHasta.HasValue ? h.DomHasta.Value.ToString("hh\\:mm") : "",
                            RecesoComida = h.IndComida ? "Sí" : "No",
                            ComidaDesde = h.ComidaDesde.HasValue ? h.ComidaDesde.Value.ToString("hh\\:mm") : "",
                            ComidaHasta = h.ComidaHasta.HasValue ? h.ComidaHasta.Value.ToString("hh\\:mm") : "",
                            TotalHorasSemana = h.TotalHorasSemana,
                            Activo = h.Activo ? "Sí" : "No"
                           
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
            string fileName = "Horarios.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Horarios");
                worksheet.Columns().AdjustToContents(); // Ajustar el ancho de las columnas automáticamente

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }


        // GET: Horario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horario = await _context.Horarios
                .Include(h => h.IdTipoHorarioNavigation)
                .FirstOrDefaultAsync(m => m.IdHorario == id);
            if (horario == null)
            {
                return NotFound();
            }
            ViewData["IdTipoHorario"] = new SelectList(_context.TipoHorarios, "IdTipoHorario", "NombreTipoHorario");
            return View(horario);
        }

        // GET: Horario/Create
        public IActionResult Create()
        {
            ViewData["IdTipoHorario"] = new SelectList(_context.TipoHorarios,"IdTipoHorario","NombreTipoHorario");
            return View();
        }

        // POST: Horario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHorario,NombreHorario,IdTipoHorario,TurnoNumero,IndLunes,IndMartes,IndMiercoles,IndJueves,IndViernes,IndSabado,IndDomingo,LunDesde,LunHasta,MarDesde,MarHasta,MieDesde,MieHasta,JueDesde,JueHasta,VieDesde,VieHasta,SabDesde,SabHasta,DomDesde,DomHasta,IndComida,ComidaDesde,ComidaHasta,TotalHorasSemana,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Horario horario)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(horario, true);
                _context.Add(horario);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdTipoHorario"] = new SelectList(_context.TipoHorarios,"IdTipoHorario","NombreTipoHorario", horario.IdTipoHorario);
            return View(horario);
        }

        // GET: Horario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horario = await _context.Horarios.FindAsync(id);
            if (horario == null)
            {
                return NotFound();
            }
            ViewData["IdTipoHorario"] = new SelectList(_context.TipoHorarios, "IdTipoHorario", "NombreTipoHorario", horario.IdTipoHorario);
            return View(horario);
        }

        // POST: Horario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHorario,NombreHorario,IdTipoHorario,TurnoNumero,IndLunes,IndMartes,IndMiercoles,IndJueves,IndViernes,IndSabado,IndDomingo,LunDesde,LunHasta,MarDesde,MarHasta,MieDesde,MieHasta,JueDesde,JueHasta,VieDesde,VieHasta,SabDesde,SabHasta,DomDesde,DomHasta,IndComida,ComidaDesde,ComidaHasta,TotalHorasSemana,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Horario horario)
        {
            if (id != horario.IdHorario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(horario, false);
                    _context.Update(horario);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HorarioExists(horario.IdHorario))
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
            ViewData["IdTipoHorario"] = new SelectList(_context.TipoHorarios, "IdTipoHorario", "NombreTipoHorario", horario.IdTipoHorario);
            return View(horario);
        }

        // GET: Horario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horario = await _context.Horarios
                .Include(h => h.IdTipoHorarioNavigation)
                .FirstOrDefaultAsync(m => m.IdHorario == id);
            if (horario == null)
            {
                return NotFound();
            }

            return View(horario);
        }

        // POST: Horario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var horario = await _context.Horarios.FindAsync(id);
            try
            {
               
                if (horario != null)
                {
                    _context.Horarios.Remove(horario);
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
                return View(horario);
            }

        }

        private bool HorarioExists(int id)
        {
            return _context.Horarios.Any(e => e.IdHorario == id);
        }
        
        private void SetCamposAuditoria(Horario record, bool bNewRecord)
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
