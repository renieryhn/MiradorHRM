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
using static PlanillaPM.Models.Viatico;
using static PlanillaPM.Models.Nomina;

namespace PlanillaPM.Controllers
{
    public class NominaController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public NominaController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Nomina
        
        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, string? idTipoNomina, int? estado)
        {
            IQueryable<Nomina> query = _context.Nominas;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdTipoNominaNavigation.NombreTipoNomina.ToLower().Contains(filter.ToLower()));
            }
            if (!String.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdNomina.ToString().Contains(idEmpleado));
            }
            if (!String.IsNullOrEmpty(idTipoNomina))
            {
                query = query.Where(r => r.IdTipoNomina.ToString().Contains(idTipoNomina));
            }

            if (estado.HasValue)
            {
                if (estado == 1)
                {
                    query = query.Where(r => r.Activo == false);
                }
                else if (estado == 0)
                {
                    query = query.Where(r => r.Activo == true);
                }
                // No hace falta ningún filtro si el estado es null o no es 0 ni 1 (es decir, se quieren mostrar todos los registros)
            }

            ViewBag.Filter = filter;
            ViewBag.CurrentIdEmpleado = idEmpleado;
            ViewBag.CurrentIdTipoNomina = idTipoNomina;
            ViewBag.CurrentEstado = estado;
         

            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = query.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = query.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            if (idEmpleado != null)
            {
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            }
            else
            {
                ViewData["IdEmpleado"] = new SelectList(IdEmpleadoNavigation, "IdEmpleado", "NombreCompleto");
            }

            var IdTipoNomina = await _context.TipoNominas.ToListAsync();
            ViewBag.IdTipoNomina = new SelectList(IdTipoNomina, "IdTipoNomina", "NombreTipoNomina");

            return View(data);

        }


       

        [HttpGet]
        public ActionResult DownloadAll(string? filter, string? idEmpleado, string? idTipoNomina, int? estado)
        {
            IQueryable<Nomina> query = _context.Nominas
                .Include(n => n.IdTipoNominaNavigation);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdTipoNominaNavigation.NombreTipoNomina.ToLower().Contains(filter.ToLower()));
            }

            if (!string.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdNomina.ToString().Contains(idEmpleado));
            }

            if (!string.IsNullOrEmpty(idTipoNomina))
            {
                query = query.Where(r => r.IdTipoNomina.ToString().Contains(idTipoNomina));
            }

            if (estado.HasValue)
            {
                if (estado == 1)
                {
                    query = query.Where(r => r.Activo == false);
                }
                else if (estado == 0)
                {
                    query = query.Where(r => r.Activo == true);
                }
            }

            var data = query
                .Select(n => new
                {
                    n.IdNomina,
                    TipoNomina = n.IdTipoNominaNavigation.NombreTipoNomina,
                    n.Comentarios,
                    n.PeriodoFiscal,
                    Mes = n.Mes.ToString(),
                    FechaPago = n.FechaPago.ToString("dd/MM/yyyy"),
                    n.TotalIngresos,
                    n.TotalDeducciones,
                    n.TotalImpuestos,
                    n.TotalEmpleadosEnNomina,
                    n.PagoNeto,
                    EstadoNomina = n.EstadoNomina.ToString(),
                    n.AprobadaPor,
                    n.ComentariosAprobador,
                    Activo = n.Activo ? "Sí" : "No"
                })
                .ToList();

            if (!data.Any())
            {
                TempData["error"] = "No se encontraron nóminas con los filtros aplicados.";
                return RedirectToAction(nameof(Index), new { filter, idEmpleado, idTipoNomina, estado });
            }

            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            string fileName = "Nominas.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Nóminas");
                worksheet.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                fileName);
                }
            }
        }


        // GET: Nomina/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomina = await _context.Nominas
                .Include(n => n.IdTipoNominaNavigation)
                .FirstOrDefaultAsync(m => m.IdNomina == id);
            if (nomina == null)
            {
                return NotFound();
            }

            return View(nomina);
        }

        // GET: Nomina/Create
        public IActionResult Create()
        {
            ViewBag.NominaEstado = Enum.GetValues(typeof(NominaEstado));
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas.Where(r => r.Activo), "IdTipoNomina", "NombreTipoNomina");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNomina,IdTipoNomina,Comentarios,PeriodoFiscal,Mes,FechaPago,TotalIngresos,TotalDeducciones,TotalImpuestos,TotalEmpleadosEnNomina,PagoNeto,EstadoNomina,AprobadaPor,ComentariosAprobador,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Nomina nomina)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(nomina, true);
                    _context.Add(nomina);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["error"] = "Error: " + message;
                }
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null && dbEx.InnerException.Message.Contains("IX_Nomina"))
                {
                    TempData["error"] = "No se puede repetir el Tipo de Nómina, Periodo Fiscal y Mes. Por favor, verifique los datos.";
                }
                else
                {
                    //TempData["error"] = "Error: " + dbEx.Message;
                    TempData["error"] = "Error: " + (dbEx.InnerException?.Message ?? dbEx.Message);

                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error: " + ex.Message;
            }

            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas, "IdTipoNomina", "NombreTipoNomina", nomina.IdTipoNomina);
            return View(nomina);
        }

        // GET: Nomina/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomina = await _context.Nominas.FindAsync(id);
            if (nomina == null)
            {
                return NotFound();
            }

            ViewBag.NominaEstado = Enum.GetValues(typeof(Nomina.NominaEstado));
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas, "IdTipoNomina", "NombreTipoNomina", nomina.IdTipoNomina);
            return View(nomina);
        }

        // POST: Nomina/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNomina,IdTipoNomina,Comentarios,PeriodoFiscal,Mes,FechaPago,TotalIngresos,TotalDeducciones,TotalImpuestos,TotalEmpleadosEnNomina,PagoNeto,EstadoNomina,AprobadaPor,ComentariosAprobador,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Nomina nomina)
        {
            if (id != nomina.IdNomina)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(nomina, false);
                    _context.Update(nomina);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NominaExists(nomina.IdNomina))
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
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas, "IdTipoNomina", "NombreTipoNomina", nomina.IdTipoNomina);
            return View(nomina);
        }

        // GET: Nomina/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nomina = await _context.Nominas
                .Include(n => n.IdTipoNominaNavigation)
                .FirstOrDefaultAsync(m => m.IdNomina == id);
            if (nomina == null)
            {
                return NotFound();
            }

            return View(nomina);
        }

        // POST: Nomina/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var nomina = await _context.Nominas.FindAsync(id);
            try
            {
               
                if (nomina != null)
                {
                    _context.Nominas.Remove(nomina);
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
                return View(nomina);
            }

        }

        private bool NominaExists(int id)
        {
            return _context.Nominas.Any(e => e.IdNomina == id);
        }
        
        private void SetCamposAuditoria(Nomina record, bool bNewRecord)
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
