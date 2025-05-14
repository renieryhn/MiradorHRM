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
using static PlanillaPM.Models.EmpleadoDeduccion;


namespace PlanillaPM.Controllers
{
    public class EmpleadoDeduccionController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoDeduccionController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }   

        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, string? idDeduccion, int? estado)
        {
            IQueryable<EmpleadoDeduccion> query = _context.EmpleadoDeduccions;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdDeduccionNavigation.NombreDeduccion.ToLower().Contains(filter.ToLower()));
            }
            if (!String.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdEmpleado.ToString().Contains(idEmpleado));
            }
            if (!String.IsNullOrEmpty(idDeduccion))
            {
                query = query.Where(r => r.IdDeduccion.ToString().Contains(idDeduccion));
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
            ViewBag.CurrentIdidDeduccion = idDeduccion;
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
            var IdDeduccionNavigation = await _context.Deduccions.ToListAsync();
            ViewBag.IdDeduccion = new SelectList(IdDeduccionNavigation, "IdDeduccion", "NombreDeduccion");         
         

            return View(data);

        }
        [HttpGet]
        public ActionResult Download(string? filter, string? idEmpleado, string? idDeduccion, int? estado)
        {
            IQueryable<EmpleadoDeduccion> query = _context.EmpleadoDeduccions
                .Include(ed => ed.IdEmpleadoNavigation)
                .Include(ed => ed.IdDeduccionNavigation);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(ed => ed.IdDeduccionNavigation.NombreDeduccion.ToLower().Contains(filter.ToLower()));
            }
            if (!string.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(ed => ed.IdEmpleado.ToString().Contains(idEmpleado));
            }
            if (!string.IsNullOrEmpty(idDeduccion))
            {
                query = query.Where(ed => ed.IdDeduccion.ToString().Contains(idDeduccion));
            }

            if (estado.HasValue)
            {
                query = estado == 1
                    ? query.Where(ed => !ed.Activo)
                    : query.Where(ed => ed.Activo);
            }

            var data = query.Select(ed => new
            {
                ed.IdEmpleadoDeduccion,
                ed.IdEmpleado,
                EmpleadoNombre = ed.IdEmpleadoNavigation.NombreEmpleado,
                ed.IdDeduccion,
                DeduccionNombre = ed.IdDeduccionNavigation.NombreDeduccion,
                Tipo = ed.Tipo.ToString(),
                ed.Monto,
                ed.Formula,
                ed.Orden,
                Activo = ed.Activo ? "Sí" : "No"
            }).ToList();

            if (!data.Any())
            {
                TempData["error"] = "No se encontraron registros de deducciones de empleados con los filtros aplicados.";
                return RedirectToAction(nameof(Index), new { filter, idEmpleado, idDeduccion, estado });
            }

            var table = new ListtoDataTableConverter().ToDataTable(data);
            using var wb = new XLWorkbook();
            wb.Worksheets.Add(table, "Empleado Deducciones");

            using var stream = new MemoryStream();
            wb.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EmpleadoDeduccions.xlsx");
        }



        [HttpGet]
        public ActionResult DownloadAll(string? filter, string? idEmpleado, string? idDeduccion, int? estado)
        {
            IQueryable<EmpleadoDeduccion> query = _context.EmpleadoDeduccions
                .Include(ed => ed.IdEmpleadoNavigation)
                .Include(ed => ed.IdDeduccionNavigation);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.IdDeduccionNavigation.NombreDeduccion.ToLower().Contains(filter.ToLower()));
            }

            if (!string.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdEmpleado.ToString().Contains(idEmpleado));
            }

            if (!string.IsNullOrEmpty(idDeduccion))
            {
                query = query.Where(r => r.IdDeduccion.ToString().Contains(idDeduccion));
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
                .Select(ed => new
                {
                    ed.IdEmpleadoDeduccion,
                    ed.IdEmpleado,
                    EmpleadoNombre = ed.IdEmpleadoNavigation.NombreEmpleado,
                    ed.IdDeduccion,
                    DeduccionNombre = ed.IdDeduccionNavigation.NombreDeduccion,
                    Tipo = ed.Tipo.ToString(),
                    ed.Monto,
                    ed.Formula,
                    ed.Orden,
                    Activo = ed.Activo ? "Sí" : "No"
                })
                .ToList();

            if (!data.Any())
            {
                TempData["error"] = "No se encontraron registros de deducciones de empleados con los filtros aplicados.";
                return RedirectToAction(nameof(Index), new { filter, idEmpleado, idDeduccion, estado });
            }

            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            string fileName = "EmpleadoDeduccions.xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table, "Deducciones");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }



        // GET: EmpleadoDeduccion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoDeduccion = await _context.EmpleadoDeduccions
                .Include(e => e.IdDeduccionNavigation)
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoDeduccion == id);
            if (empleadoDeduccion == null)
            {
                return NotFound();
            }

            return View(empleadoDeduccion);
        }

        // GET: EmpleadoDeduccion/Create
        public  IActionResult Create(int? id, int? idEmpleado)
        {
            ViewBag.TipoEstado = Enum.GetValues(typeof(TipoEstado))
                             .Cast<TipoEstado>()
                             .Select(e => new SelectListItem
                             {
                                 Value = ((int)e).ToString(),
                                 Text = e.ToString()
                             }).ToList();
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion");
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");          
            return View();
        }

        public JsonResult GetIngresoDetails(int id)
        {

            var deduccion = _context.Deduccions
                .Where(i => i.IdDeduccion == id)
                .Select(i => new
                {
                    montoIngreso = i.Monto,
                    formulaIngreso = i.Formula,
                    ordenIngreso = i.Orden,
                    tipoIngreso = i.TipoDeduccion

                })
                .FirstOrDefault();

            if (deduccion == null)
            {
                return Json(new { error = "Ingreso no encontrado" });
            }

            return Json(deduccion);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("IdEmpleadoDeduccion,IdEmpleado,IdDeduccion,Tipo,Monto,Formula,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoDeduccion empleadoDeduccion, int? id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        SetCamposAuditoria(empleadoDeduccion, true);
        //        _context.Add(empleadoDeduccion);
        //        await _context.SaveChangesAsync();
        //        TempData["success"] = "El registro ha sido creado exitosamente.";

        //        if (id.HasValue)
        //        {
        //            if (id == 1)
        //            {
        //                return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoDeduccion.IdEmpleado}?tab=settings");
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
        //    ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", empleadoDeduccion.IdDeduccion);
        //    ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoDeduccion.IdEmpleado);
        //    // Si el modelo no es válido o si no se encuentra el valor de dirección, siempre redirigir.
        //    if (id.HasValue)
        //    {
        //        if (id == 1)
        //        {
        //            return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoDeduccion.IdEmpleado}?tab=settings");
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
        //    return View(empleadoDeduccion);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoDeduccion,IdEmpleado,IdDeduccion,Tipo,Monto,Formula,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoDeduccion empleadoDeduccion, int? id)
        {
            try
            {
                // Validar si ya existe esa combinación
                bool yaExiste = await _context.EmpleadoDeduccions
                    .AnyAsync(d => d.IdEmpleado == empleadoDeduccion.IdEmpleado && d.IdDeduccion == empleadoDeduccion.IdDeduccion);

                if (yaExiste)
                {
                    TempData["error"] = "Esta deducción ya ha sido asignada al empleado.";
                    ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", empleadoDeduccion.IdDeduccion);
                    ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoDeduccion.IdEmpleado);
                    return View(empleadoDeduccion);
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoDeduccion, true);
                    _context.Add(empleadoDeduccion);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido creado exitosamente.";

                    if (id == 1)
                        return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoDeduccion.IdEmpleado}?tab=settings");
                    if (id == 2)
                        return RedirectToAction("Index");
                }
                else
                {
                    var errores = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["error"] = "Error: " + errores;
                }
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Error: ya existe esta deducción para el empleado.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error inesperado: " + ex.Message;
            }

            // Cargar combos y retornar vista si algo falló
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", empleadoDeduccion.IdDeduccion);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoDeduccion.IdEmpleado);
            return View(empleadoDeduccion);
        }


        // GET: EmpleadoDeduccion/Edit/5
        public async Task<IActionResult> Edit(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoDeduccion = await _context.EmpleadoDeduccions.FindAsync(id);
            if (empleadoDeduccion == null)
            {
                return NotFound();
            }
            ViewBag.TipoEstado = Enum.GetValues(typeof(EmpleadoDeduccion.TipoEstado));
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", empleadoDeduccion.IdDeduccion);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoDeduccion.IdEmpleado);
            ViewBag.Numero = numero;
            return View(empleadoDeduccion);
        }

        // POST: EmpleadoDeduccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoDeduccion,IdEmpleado,IdDeduccion,Tipo,Monto,Formula,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoDeduccion empleadoDeduccion, string? numero)
        {
            if (id != empleadoDeduccion.IdEmpleadoDeduccion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(empleadoDeduccion, false);
                    _context.Update(empleadoDeduccion);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoDeduccionExists(empleadoDeduccion.IdEmpleadoDeduccion))
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
                    return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoDeduccion.IdEmpleado}?tab=settings");
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
            ViewData["IdDeduccion"] = new SelectList(_context.Deduccions, "IdDeduccion", "NombreDeduccion", empleadoDeduccion.IdDeduccion);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoDeduccion.IdEmpleado);
            return View(empleadoDeduccion);
        }

        // GET: EmpleadoDeduccion/Delete/5
        public async Task<IActionResult> Delete(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoDeduccion = await _context.EmpleadoDeduccions
                .Include(e => e.IdDeduccionNavigation)
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoDeduccion == id);
            if (empleadoDeduccion == null)
            {
                return NotFound();
            }

            ViewBag.Numero = numero;
            return View(empleadoDeduccion);
        }

        // POST: EmpleadoDeduccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? numero)
        {
             var empleadoDeduccion = await _context.EmpleadoDeduccions.FindAsync(id);
            try
            {
               
                if (empleadoDeduccion != null)
                {
                    _context.EmpleadoDeduccions.Remove(empleadoDeduccion);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido eliminado exitosamente.";
                   
                } 
                else
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                   
                }
                if (numero == "1")
                {
                    return Redirect($"/NominaEmpleado/IDIEmpleado/{empleadoDeduccion.IdEmpleado}?tab=settings");
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
                return View(empleadoDeduccion);
            }

        }

        private bool EmpleadoDeduccionExists(int id)
        {
            return _context.EmpleadoDeduccions.Any(e => e.IdEmpleadoDeduccion == id);
        }
        
        private void SetCamposAuditoria(EmpleadoDeduccion record, bool bNewRecord)
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
