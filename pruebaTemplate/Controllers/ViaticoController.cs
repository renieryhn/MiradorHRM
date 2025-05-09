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
using static PlanillaPM.Models.EmpleadoContrato;
using static PlanillaPM.Models.Viatico;

namespace PlanillaPM.Controllers
{
    public class ViaticoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ViaticoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, int? estado)
        {
            //IQueryable<Viatico> query = _context.Viaticos;
            IQueryable<Viatico> query = _context.Viaticos
    .Include(v => v.IdEmpleadoNavigation);

            if (!String.IsNullOrEmpty(filter))
            {
                //query = query.Where(r => r.IdEmpleadoNavigation.NombreCompleto.ToLower().Contains(filter.ToLower()));
                query = query.Where(r =>
        r.IdEmpleadoNavigation.NombreEmpleado.ToLower().Contains(filter.ToLower()) ||
        r.IdEmpleadoNavigation.ApellidoEmpleado.ToLower().Contains(filter.ToLower()));
            }
            if (!String.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdEmpleado.ToString().Contains(idEmpleado));
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
            var planillaContext = await _context.EmpleadoDeduccions.ToListAsync();

            return View(data);

        }
        public ActionResult Download()
        {
            // Obtener la lista de todos los viáticos
            var data = _context.Viaticos
                        .Select(v => new
                        {
                            v.IdViatico,
                            Empleado = v.IdEmpleadoNavigation.NombreCompleto, // Asumiendo que hay una propiedad que contiene el nombre del empleado
                            v.Descripcion,
                            Fecha = v.Fecha.ToString("dd/MM/yyyy"),
                            TotalGastos = v.TotalGastos.ToString("C"),
                            AdelantoRecibido = v.AdelantoRecibido.ToString("C"),
                            BalancePendiente = v.BalancePendiente.ToString("C"),
                            Estado = v.Estado.ToString(), // Convertir el enum en cadena
                            FechaAprobacion = v.FechaAprobacion.HasValue ? v.FechaAprobacion.Value.ToString("dd/MM/yyyy") : "",
                            v.AprobadoPor,
                            v.NotasAdicionales,
                            Pagado = v.Pagado ? "Sí" : "No",
                            FechaPago = v.FechaPago.HasValue ? v.FechaPago.Value.ToString("dd/MM/yyyy") : "",
                            Activo = v.Activo ? "Sí" : "No"
                           
                        })
                        .ToList();

            // Verificar si hay datos
            if (!data.Any())
            {
                TempData["error"] = "No se encontraron viáticos.";
                return RedirectToAction(nameof(Index));
            }

            // Convertir la lista en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo Excel
            string fileName = "Viaticos.xlsx";

            // Crear el archivo de Excel
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Viaticos");
                worksheet.Columns().AdjustToContents(); // Ajustar el ancho de las columnas automáticamente

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // GET: Viatico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatico = await _context.Viaticos
                .Include(v => v.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdViatico == id);
            if (viatico == null)
            {
                return NotFound();
            }

            return View(viatico);
        }

        // GET: Viatico/Create
        public IActionResult Create()
        {
            ViewBag.TipoEstado = Enum.GetValues(typeof(TipoEstado));
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            return View();
        }

        // POST: Viatico/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdViatico,IdEmpleado,Descripcion,Fecha,TotalGastos,AdelantoRecibido,BalancePendiente,Estado,FechaAprobacion,AprobadoPor,NotasAdicionales,Pagado,FechaPago,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Viatico viatico)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(viatico, true);
                _context.Add(viatico);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", viatico.IdEmpleado);
            return View(viatico);
        }

        // GET: Viatico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatico = await _context.Viaticos.FindAsync(id);
            if (viatico == null)
            {
                return NotFound();
            }
            ViewBag.TipoEstado = Enum.GetValues(typeof(Viatico.TipoEstado));
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", viatico.IdEmpleado);
            return View(viatico);
        }

        // POST: Viatico/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdViatico,IdEmpleado,Descripcion,Fecha,TotalGastos,AdelantoRecibido,BalancePendiente,Estado,FechaAprobacion,AprobadoPor,NotasAdicionales,Pagado,FechaPago,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Viatico viatico)
        {
            if (id != viatico.IdViatico)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(viatico, false);
                    _context.Update(viatico);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViaticoExists(viatico.IdViatico))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", viatico.IdEmpleado);
            return View(viatico);
        }

        // GET: Viatico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viatico = await _context.Viaticos
                .Include(v => v.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdViatico == id);
            if (viatico == null)
            {
                return NotFound();
            }

            return View(viatico);
        }

        // POST: Viatico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var viatico = await _context.Viaticos.FindAsync(id);
            try
            {
               
                if (viatico != null)
                {
                    _context.Viaticos.Remove(viatico);
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
                return View(viatico);
            }

        }

        private bool ViaticoExists(int id)
        {
            return _context.Viaticos.Any(e => e.IdViatico == id);
        }
        
        private void SetCamposAuditoria(Viatico record, bool bNewRecord)
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
