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
    public class EmpleadoImpuestoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoImpuestoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoImpuesto
      
        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, string? idImpuesto, int? estado)
        {
            IQueryable<EmpleadoImpuesto> query = _context.EmpleadoImpuestos;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdImpuestoNavigation.NombreImpuesto.ToLower().Contains(filter.ToLower()));
            }
            if (!String.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdEmpleado.ToString().Contains(idEmpleado));
            }
            if (!String.IsNullOrEmpty(idImpuesto))
            {
                query = query.Where(r => r.IdImpuesto.ToString().Contains(idImpuesto));
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
            ViewBag.CurrentIdidImpuesto = idImpuesto;
            ViewBag.CurrentEstado = estado;


            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = query.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = query.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            //var planillaContext = await _context.EmpleadoImpuestos
            //    .Include(e => e.IdEmpleadoNavigation)
            //    .Include(e => e.IdImpuestoNavigation);

            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            if (idEmpleado != null)
            {
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            }
            else
            {
                ViewData["IdEmpleado"] = new SelectList(IdEmpleadoNavigation, "IdEmpleado", "NombreCompleto");
            }
           
            var IdImpuestoNavigation = await _context.Impuestos.ToListAsync();
            ViewBag.IdImpuesto = new SelectList(IdImpuestoNavigation, "IdImpuesto", "NombreImpuesto");

          

            

            return View(data);

        }

        public IActionResult Download(string? filter, string? idEmpleado, string? idImpuesto, int? estado)
        {
            IQueryable<EmpleadoImpuesto> query = _context.EmpleadoImpuestos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdImpuestoNavigation);

            if (!string.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(e => e.IdEmpleado.ToString().Contains(idEmpleado));
            }

            if (!string.IsNullOrEmpty(idImpuesto))
            {
                query = query.Where(e => e.IdImpuesto.ToString().Contains(idImpuesto));
            }

            if (estado.HasValue)
            {
                if (estado == 1)
                    query = query.Where(e => !e.Activo);
                else if (estado == 0)
                    query = query.Where(e => e.Activo);
            }

            var data = query.Select(e => new
            {
                e.IdEmpleadoImpuesto,
                e.IdImpuesto,
                ImpuestoNombre = e.IdImpuestoNavigation.NombreImpuesto,
                e.IdEmpleado,
                EmpleadoNombre = e.IdEmpleadoNavigation.NombreCompleto,
                Exento = e.Excento ? "Sí" : "No",
                e.Orden,
                Activo = e.Activo ? "Sí" : "No"
            }).ToList();

            if (!data.Any())
            {
                TempData["error"] = "No se encontraron registros de empleado impuesto con los filtros aplicados.";
                return RedirectToAction(nameof(Index), new { filter, idEmpleado, idImpuesto, estado });
            }

            DataTable table = new ListtoDataTableConverter().ToDataTable(data);
            using var wb = new XLWorkbook();
            wb.Worksheets.Add(table, "Empleado Impuestos");

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EmpleadoImpuestos.xlsx");
        }


        [HttpGet]
        public ActionResult DownloadAll(string? filter, string? idEmpleado, string? idImpuesto, int? estado)
        {
            IQueryable<EmpleadoImpuesto> query = _context.EmpleadoImpuestos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdImpuestoNavigation);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdImpuestoNavigation.NombreImpuesto.ToLower().Contains(filter.ToLower()));
            }

            if (!string.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdEmpleado.ToString().Contains(idEmpleado));
            }

            if (!string.IsNullOrEmpty(idImpuesto))
            {
                query = query.Where(r => r.IdImpuesto.ToString().Contains(idImpuesto));
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
                .Select(e => new
                {
                    e.IdEmpleadoImpuesto,
                    e.IdImpuesto,
                    ImpuestoNombre = e.IdImpuestoNavigation.NombreImpuesto,
                    e.IdEmpleado,
                    EmpleadoNombre = e.IdEmpleadoNavigation.NombreCompleto,
                    Exento = e.Excento ? "Sí" : "No",
                    e.Orden,
                    Activo = e.Activo ? "Sí" : "No"
                })
                .ToList();

            if (!data.Any())
            {
                TempData["error"] = "No se encontraron registros de empleado impuesto con los filtros aplicados.";
                return RedirectToAction(nameof(Index), new { filter, idEmpleado, idImpuesto, estado });
            }

            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);
            string fileName = "EmpleadoImpuestos.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table, "Empleado Impuestos");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }



        // GET: EmpleadoImpuesto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoImpuesto = await _context.EmpleadoImpuestos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdImpuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoImpuesto == id);
            if (empleadoImpuesto == null)
            {
                return NotFound();
            }

            return View(empleadoImpuesto);
        }

        // GET: EmpleadoImpuesto/Create
        public IActionResult Create(int? id, int? idEmpleado)
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto");
            return View();
        }

        public JsonResult GetIngresoDetails(int id)
        {

            var impuesto = _context.Impuestos
                .Where(i => i.IdImpuesto == id)
                .Select(i => new
                {                  
                    ordenIngreso = i.Orden
                    
                })
                .FirstOrDefault();

            if (impuesto == null)
            {
                return Json(new { error = "Ingreso no encontrado" });
            }

            return Json(impuesto);
        }

        // POST: EmpleadoImpuesto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("IdEmpleadoImpuesto,IdImpuesto,IdEmpleado,Excento,Orden,FechaCreacion,Activo,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoImpuesto empleadoImpuesto, int? id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        SetCamposAuditoria(empleadoImpuesto, true);
        //        _context.Add(empleadoImpuesto);
        //        await _context.SaveChangesAsync();
        //        TempData["success"] = "El registro ha sido creado exitosamente.";

        //        if (id.HasValue)
        //        {
        //            if (id == 1)
        //            {
        //                return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoImpuesto.IdEmpleado}?tab=messages");
        //            }
        //            if (id == 2)
        //            {
        //                return RedirectToAction("Index");
        //            }
        //        }
        //        else
        //        {
        //            TempData["error"] = "Error: No se encontró el valor de la dirección.";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    else
        //    {
        //        var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        //        TempData["error"] = "Error: " + message;
        //    }
        //    ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoImpuesto.IdEmpleado);
        //    ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto", empleadoImpuesto.IdImpuesto);

        //    // Si el modelo no es válido o si no se encuentra el valor de dirección, siempre redirigir.
        //    if (id.HasValue)
        //    {
        //        if (id == 1)
        //        {
        //            return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoImpuesto.IdEmpleado}?tab=messages");
        //        }
        //        if (id == 2)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    else
        //    {
        //        TempData["error"] = "Error: No se encontró el valor de la dirección.";
        //        return RedirectToAction("Index");
        //    }
        //    return View(empleadoImpuesto);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoImpuesto,IdImpuesto,IdEmpleado,Excento,Orden,FechaCreacion,Activo,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoImpuesto empleadoImpuesto, int? id)
        {
            try
            {
                // Verifica si ya existe esa combinación
                bool yaExiste = await _context.EmpleadoImpuestos
                    .AnyAsync(e => e.IdEmpleado == empleadoImpuesto.IdEmpleado && e.IdImpuesto == empleadoImpuesto.IdImpuesto);

                if (yaExiste)
                {
                    TempData["error"] = "Ya existe un registro con este empleado e impuesto.";
                    ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoImpuesto.IdEmpleado);
                    ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto", empleadoImpuesto.IdImpuesto);
                    return View(empleadoImpuesto);
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoImpuesto, true);
                    _context.Add(empleadoImpuesto);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";

                    if (id == 1)
                        return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoImpuesto.IdEmpleado}?tab=messages");

                    if (id == 2)
                        return RedirectToAction("Index");
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["error"] = "Error: " + message;
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["error"] = "Error al guardar los datos: posible clave duplicada u otro problema de base de datos.";
                // Opcional: log ex.InnerException?.Message para más detalles
            }
            catch (Exception ex)
            {
                TempData["error"] = "Ocurrió un error inesperado. " + ex.Message;
            }

            // En caso de error, volver a cargar combos y mostrar la vista
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoImpuesto.IdEmpleado);
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto", empleadoImpuesto.IdImpuesto);

            return View(empleadoImpuesto);
        }


        // GET: EmpleadoImpuesto/Edit/5
        public async Task<IActionResult> Edit(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoImpuesto = await _context.EmpleadoImpuestos.FindAsync(id);
            if (empleadoImpuesto == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoImpuesto.IdEmpleado);
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto", empleadoImpuesto.IdImpuesto);
            ViewBag.Numero = numero;
            return View(empleadoImpuesto);
        }

        // POST: EmpleadoImpuesto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoImpuesto,IdImpuesto,IdEmpleado,Excento,Orden,FechaCreacion,Activo,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoImpuesto empleadoImpuesto, string? numero)
        {
            if (id != empleadoImpuesto.IdEmpleadoImpuesto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(empleadoImpuesto, false);
                    _context.Update(empleadoImpuesto);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoImpuestoExists(empleadoImpuesto.IdEmpleadoImpuesto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                

                if (numero == "1")
                {
                    return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoImpuesto.IdEmpleado}?tab=messages");
                }
                if (numero == "2")
                {
                    return RedirectToAction("Index");
                }
            }            
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoImpuesto.IdEmpleado);
            ViewData["IdImpuesto"] = new SelectList(_context.Impuestos, "IdImpuesto", "NombreImpuesto", empleadoImpuesto.IdImpuesto);
            return View(empleadoImpuesto);
        }

        // GET: EmpleadoImpuesto/Delete/5
        public async Task<IActionResult> Delete(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoImpuesto = await _context.EmpleadoImpuestos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdImpuestoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoImpuesto == id);
            if (empleadoImpuesto == null)
            {
                return NotFound();
            }

            ViewBag.Numero = numero;
            return View(empleadoImpuesto);
        }

        // POST: EmpleadoImpuesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? numero)
        {
             var empleadoImpuesto = await _context.EmpleadoImpuestos.FindAsync(id);
            try
            {
               
                if (empleadoImpuesto != null)
                {
                    _context.EmpleadoImpuestos.Remove(empleadoImpuesto);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido eliminado exitosamente.";
                   
                } 
                else
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                   
                }

                if (numero == "1")
                {
                    return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoImpuesto.IdEmpleado}?tab=messages");
                }
                if (numero == "2")
                {
                    return RedirectToAction("Index");
                }

                // Si no se cumple ninguna condición anterior, redirige a una página predeterminada
                return RedirectToAction("Index");
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
                return View(empleadoImpuesto);
            }

        }

        private bool EmpleadoImpuestoExists(int id)
        {
            return _context.EmpleadoImpuestos.Any(e => e.IdEmpleadoImpuesto == id);
        }
        
        private void SetCamposAuditoria(EmpleadoImpuesto record, bool bNewRecord)
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
