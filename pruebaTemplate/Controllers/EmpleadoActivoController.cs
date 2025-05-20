using System.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static PlanillaPM.cGeneralFun;
using PlanillaPM.Models;

using static PlanillaPM.Models.EmpleadoActivo;

namespace PlanillaPM.Controllers
{
    public class EmpleadoActivoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoActivoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoActivo
        public async Task<IActionResult> Index(int pg, string? filter,string? idEmpleado, int? estado)
        {
            
            IQueryable<EmpleadoActivo> query = _context.EmpleadoActivos;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.NumeroSerie.ToLower().Contains(filter.ToLower()));
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
            //ViewBag.CurrentEstado = estado;

            List<EmpleadoActivo> registros;
            registros = await query.Include(e => e.IdEmpleadoNavigation).ToListAsync();

            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
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

            var planillaContext = _context.EmpleadoActivos.Include(e => e.IdEmpleadoNavigation).Include(e => e.IdProductoNavigation);           
            var IdProductoNavigation = await _context.Productos.ToListAsync();
            return View(data);
        }

        [HttpGet]
        public ActionResult Download(int id, string? filter)
        {
            // Construir la consulta base
            var query = _context.EmpleadoActivos
                .Include(ea => ea.IdEmpleadoNavigation)
                .Include(ea => ea.IdProductoNavigation)
                .Where(ea => ea.IdEmpleado == id);

            // Aplicar filtro si existe
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(ea => ea.NumeroSerie.Contains(filter));
            }

            // Obtener los datos
            var data = query
                .Select(ea => new
                {
                    ea.IdEmpleadoActivo,
                    Empleado = ea.IdEmpleadoNavigation.NombreEmpleado + " " + ea.IdEmpleadoNavigation.ApellidoEmpleado,
                    Producto = ea.IdProductoNavigation.NombreProducto,
                    ea.Model,
                    ea.NumeroSerie,
                    Estado = ea.Estado.ToString(),
                    ea.Cantidad,
                    ea.PrecioEstimado,
                    FechaAsignacion = ea.FechaAsignacion.ToString("yyyy-MM-dd"),
                    ea.Descripcion,
                    Activo = ea.Activo ? "Sí" : "No"
                })
                .ToList();

            // Convertir a DataTable
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo
            string fileName = $"EmpleadoActivo_{id}.xlsx";

            // Generar y devolver el archivo
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table, "EmpleadoActivo");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }



        [HttpGet]
        public ActionResult DownloadAll(string? filter, string? idEmpleado, int? estado)
        {
            IQueryable<EmpleadoActivo> query = _context.EmpleadoActivos
                .Include(ea => ea.IdEmpleadoNavigation)
                .Include(ea => ea.IdProductoNavigation);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.NumeroSerie.ToLower().Contains(filter.ToLower()));
            }

            if (!string.IsNullOrEmpty(idEmpleado))
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
            }

            var data = query
                .Select(ea => new
                {
                    ea.IdEmpleadoActivo,
                    Empleado = ea.IdEmpleadoNavigation.NombreEmpleado + " " + ea.IdEmpleadoNavigation.ApellidoEmpleado,
                    Producto = ea.IdProductoNavigation.NombreProducto,
                    ea.Model,
                    ea.NumeroSerie,
                    Estado = ea.Estado.ToString(),
                    ea.Cantidad,
                    ea.PrecioEstimado,
                    FechaAsignacion = ea.FechaAsignacion.ToString("yyyy-MM-dd"),
                    ea.Descripcion,
                    Activo = ea.Activo ? "Sí" : "No"
                })
                .ToList();

            if (!data.Any())
            {
                TempData["error"] = "No se encontraron registros con los filtros aplicados.";
                return RedirectToAction(nameof(Index), new { filter, idEmpleado, estado });
            }

            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            string fileName = "EmpleadosActivos.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table, "EmpleadosActivos");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                fileName);
                }
            }
        }


        // GET: EmpleadoActivo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoActivo = await _context.EmpleadoActivos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoActivo == id);
            if (empleadoActivo == null)
            {
                return NotFound();
            }

            return View(empleadoActivo);
        }

        // GET: EmpleadoActivo/Create
        public IActionResult Create(int? id, int? idEmpleado)
        {
            ViewBag.EstadoActual = Enum.GetValues(typeof(EstadoActual));
            //ViewBag.EstadoOpcion = new SelectList(Enum.GetValues(typeof(EstadoActual)));
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto");
            return View();
        }

        // POST: EmpleadoActivo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoActivo,IdEmpleado,IdProducto,Model,NumeroSerie,Estado,Cantidad,PrecioEstimado,FechaAsignacion,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoActivo empleadoActivo, int? id)
        {
            if (ModelState.IsValid)
            {
                // Realizar las operaciones necesarias para guardar el modelo en la base de datos
                SetCamposAuditoria(empleadoActivo, true);
                _context.Add(empleadoActivo);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";


                if (id.HasValue)
                {
                    
                    if (id == 1)
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoActivo.IdEmpleado}?tab=Empleado");
                    }
                    if (id == 2)
                    {
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    TempData["error"] = "Error no se encontro el valor de la direccion";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoActivo.IdEmpleado);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", empleadoActivo.IdProducto);
            return View(empleadoActivo);
        }

        // GET: EmpleadoActivo/Edit/5
        public async Task<IActionResult> Edit(int? id, string? numero)
        {

            if (id == null)
            {
                return NotFound();
            }

            var empleadoActivo = await _context.EmpleadoActivos.FindAsync(id);

            if (empleadoActivo == null)
            {
                return NotFound();
            }

            ViewBag.EstadoActual = Enum.GetValues(typeof(EmpleadoActivo.EstadoActual));
            ViewBag.Numero = numero;
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoActivo.IdEmpleado);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", empleadoActivo.IdProducto);
            return View(empleadoActivo);
        }

        // POST: EmpleadoActivo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoActivo,IdEmpleado,IdProducto,Model,NumeroSerie,Estado,Cantidad,PrecioEstimado,FechaAsignacion,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoActivo empleadoActivo,string? numero)
        {
            if (id != empleadoActivo.IdEmpleadoActivo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(empleadoActivo, false);
                    _context.Update(empleadoActivo);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoActivoExists(empleadoActivo.IdEmpleadoActivo))
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
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoActivo.IdEmpleado}?tab=Empleado");
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoActivo.IdEmpleado);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", empleadoActivo.IdProducto);

            return View(empleadoActivo);
        }

        // GET: EmpleadoActivo/Delete/5
        public async Task<IActionResult> Delete(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoActivo = await _context.EmpleadoActivos
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoActivo == id);
            if (empleadoActivo == null)
            {
                return NotFound();
            }

            ViewBag.Numero = numero;

            return View(empleadoActivo);
        }

        // POST: EmpleadoActivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? numero)
        {
             var empleadoActivo = await _context.EmpleadoActivos.FindAsync(id);
            try
            {
               
                if (empleadoActivo != null)
                {
                    _context.EmpleadoActivos.Remove(empleadoActivo);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido eliminado exitosamente.";
                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoActivo.IdEmpleado}?tab=Empleado");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
                } 
                else
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoActivo.IdEmpleado}?tab=Empleado");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
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
                return View(empleadoActivo);
            }

            return View(empleadoActivo);
        }

        private bool EmpleadoActivoExists(int id)
        {
            return _context.EmpleadoActivos.Any(e => e.IdEmpleadoActivo == id);
        }

        private void SetCamposAuditoria(EmpleadoActivo record, bool bNewRecord)
        {
            var now = DateTime.Now;
            var CurrentUser = _userManager.GetUserName(User);

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
