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
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<EmpleadoActivo> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoActivos.Where(r => r.Model.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoActivos.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.EmpleadoActivos.Include(e => e.IdEmpleadoNavigation).Include(e => e.IdProductoNavigation);
            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            var IdProductoNavigation = await _context.Productos.ToListAsync();
            return View(data);
        }
        public ActionResult Download(int id)
        {
            // Filtrar los contactos de empleado por el id recibido
            List<EmpleadoActivo> data = _context.EmpleadoActivos.Where(ec => ec.IdEmpleado == id).ToList();

            // Convertir la lista de contactos en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = $"EmpleadoActivo{id}.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    // Devolver el archivo como una descarga de archivo Excel
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
        public IActionResult Create()
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
        public async Task<IActionResult> Create([Bind("IdEmpleadoActivo,IdEmpleado,IdProducto,Model,NumeroSerie,Estado,Cantidad,PrecioEstimado,FechaAsignacion,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoActivo empleadoActivo)
        {
            if (ModelState.IsValid)
            {
                // Realizar las operaciones necesarias para guardar el modelo en la base de datos
                SetCamposAuditoria(empleadoActivo, true);
                _context.Add(empleadoActivo);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return Redirect($"/Empleado/FichaEmpleado/{empleadoActivo.IdEmpleado}?tab=Empleado");
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
        public async Task<IActionResult> Edit(int? id)
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

            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoActivo.IdEmpleado);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", empleadoActivo.IdProducto);
            return View(empleadoActivo);
        }

        // POST: EmpleadoActivo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoActivo,IdEmpleado,IdProducto,Model,NumeroSerie,Estado,Cantidad,PrecioEstimado,FechaAsignacion,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoActivo empleadoActivo)
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
                return Redirect($"/Empleado/FichaEmpleado/{empleadoActivo.IdEmpleado}?tab=Empleado");
            }            
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", empleadoActivo.IdEmpleado);
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", empleadoActivo.IdProducto);
            return View(empleadoActivo);
        }

        // GET: EmpleadoActivo/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: EmpleadoActivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var empleadoActivo = await _context.EmpleadoActivos.FindAsync(id);
            try
            {
               
                if (empleadoActivo != null)
                {
                    _context.EmpleadoActivos.Remove(empleadoActivo);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido eliminado exitosamente.";
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoActivo.IdEmpleado}?tab=Empleado");
                } 
                else
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoActivo.IdEmpleado}?tab=Empleado");
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
