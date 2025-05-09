using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using Microsoft.AspNetCore.Identity;
using ClosedXML.Excel;
using static PlanillaPM.cGeneralFun;
using System.Data;
using static PlanillaPM.Models.EmpleadoAusencium;
using static PlanillaPM.Models.EmpleadoContrato;

namespace PlanillaPM.Controllers
{
    public class EmpleadoContratoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoContratoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoContrato
        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, int? estado)
        {

            IQueryable<EmpleadoContrato> query = _context.EmpleadoContratos;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.Descripcion.ToLower().Contains(filter.ToLower()));
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

            List<EmpleadoContrato> registros;
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

            var IdCargoNavigation = await _context.Cargos.ToListAsync();
            //var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            var IdTipoContratoNavigation = await _context.TipoContratos.ToListAsync();

            return View(data);


        }


        [HttpGet]
        public ActionResult Download(int id)
        {
            // Filtrar los contratos de empleado por el id recibido
            var data = _context.EmpleadoContratos
                        .Where(ec => ec.IdEmpleado == id)
                        .Select(ec => new
                        {
                            ec.IdEmpleadoContrato,
                            ec.CodigoContrato,
                            TipoContrato = ec.IdTipoContratoNavigation.NombreTipoContrato, // Asumiendo que tienes un campo para el nombre del tipo de contrato
                            Cargo = ec.IdCargoNavigation.NombreCargo, // Asumiendo que tienes un campo para el nombre del cargo
                            Estado = ec.Estado.ToString(),
                            VigenciaMeses = ec.VigenciaMeses,
                            FechaInicio = ec.FechaInicio.ToString("yyyy-MM-dd"),
                            FechaFin = ec.FechaFin.ToString("yyyy-MM-dd"),
                            Salario = ec.Salario.ToString("C"),
                            Descripcion = ec.Descripcion ?? "N/A",
                            Activo = ec.Activo ? "Sí" : "No"
                           
                        })
                        .ToList();

          

            // Convertir la lista de contratos en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = $"EmpleadoContrato_{id}.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "ContratosEmpleado");
                worksheet.Columns().AdjustToContents(); // Ajustar el ancho de las columnas

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    // Devolver el archivo como una descarga de archivo Excel
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        public ActionResult DownloadAll()
        {
            // Obtener todos los contratos de empleados
            var data = _context.EmpleadoContratos
                        .Select(ec => new
                        {
                            ec.IdEmpleadoContrato,
                            Empleado = ec.IdEmpleadoNavigation.NombreEmpleado + " " + ec.IdEmpleadoNavigation.ApellidoEmpleado,
                            ec.CodigoContrato,
                            TipoContrato = ec.IdTipoContratoNavigation.NombreTipoContrato, // Asumiendo que tienes un campo para el nombre del tipo de contrato
                            Cargo = ec.IdCargoNavigation.NombreCargo, // Asumiendo que tienes un campo para el nombre del cargo
                            Estado = ec.Estado.ToString(),
                            VigenciaMeses = ec.VigenciaMeses,
                            FechaInicio = ec.FechaInicio.ToString("yyyy-MM-dd"),
                            FechaFin = ec.FechaFin.ToString("yyyy-MM-dd"),
                            Salario = ec.Salario.ToString("C"),
                            Descripcion = ec.Descripcion ?? "N/A",
                            Activo = ec.Activo ? "Sí" : "No"
                            
                        })
                        .ToList();

            // Verificar si la lista está vacía
            if (!data.Any())
            {
                TempData["error"] = "No se encontraron Registros.";
                return RedirectToAction(nameof(Index));
            }
            // Convertir la lista de contratos en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = "EmpleadoContrato.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "ContratosEmpleados");
                worksheet.Columns().AdjustToContents(); // Ajustar el ancho de las columnas

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }




        // GET: EmpleadoContrato/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoContrato = await _context.EmpleadoContratos
                .Include(e => e.IdCargoNavigation)
                .Include(e => e.IdEmpleadoNavigation)
                .Include(e => e.IdTipoContratoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoContrato == id);
            if (empleadoContrato == null)
            {
                return NotFound();
            }
            return View(empleadoContrato);
        }

        // GET: EmpleadoContrato/Create
        public IActionResult Create()
        {
            ViewBag.EstadoContrato = Enum.GetValues(typeof(EstadoContrato));
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "NombreCargo");
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato");
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            return View();
        }

        // POST: EmpleadoContrato/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoContrato,IdEmpleado,CodigoContrato,IdTipoContrato,IdCargo,Estado,VigenciaMeses,FechaInicio,FechaFin,Salario,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoContrato empleadoContrato, int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoContrato, true);

                    // Buscar contratos activos del usuario
                    var contratosActivos = await _context.EmpleadoContratos
                        .Where(c => c.IdEmpleado == empleadoContrato.IdEmpleado && c.Activo)
                        .ToListAsync();

                    // Verificar si hay contratos activos
                    if (contratosActivos.Any() && empleadoContrato.Estado != EstadoContrato.Borrador)
                    {
                        TempData["Error"] = "No se puede guardar porque ya que se encuentra un contrato activo.";

                        if (id == 1)
                        {
                            return Redirect($"/Empleado/FichaEmpleado/{empleadoContrato.IdEmpleado}?tab=messages");
                        }
                        if (id == 2)
                        {
                            return RedirectToAction("Index");
                        }
                    }

                    if (empleadoContrato.Estado == EstadoContrato.Aprobado && empleadoContrato.Activo)
                    {
                        _context.Add(empleadoContrato);
                    }
                    else if ((empleadoContrato.Estado == EstadoContrato.Finalizado || empleadoContrato.Estado == EstadoContrato.Cancelado || empleadoContrato.Estado == EstadoContrato.Borrador) && empleadoContrato.Activo)
                    {
                        empleadoContrato.Activo = false;
                        _context.Add(empleadoContrato);
                    }


                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se creó exitosamente.";
                
                    
                    if (id.HasValue)
                    {

                        if (id == 1)
                        {
                            return Redirect($"/Empleado/FichaEmpleado/{empleadoContrato.IdEmpleado}?tab=messages");
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

                ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "NombreCargo", empleadoContrato.IdCargo);
                ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato", empleadoContrato.IdTipoContrato);
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoContrato.IdEmpleado);

                // Agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, verifique los datos.";

                return View(empleadoContrato);
            }
            catch (Exception ex)
            {
                

                // Agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, intente nuevamente.";

                if (id == 1)
                {
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoContrato.IdEmpleado}?tab=messages");
                }
                if (id == 2)
                {
                    return RedirectToAction("Index");
                }

                return View();
            }
        }


        // GET: EmpleadoContrato/Edit/5
        public async Task<IActionResult> Edit(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoContrato = await _context.EmpleadoContratos.FindAsync(id);
            if (empleadoContrato == null)
            {
                return NotFound();
            }

            ViewBag.Numero = numero;
            ViewBag.EstadoContrato = Enum.GetValues(typeof(EmpleadoContrato.EstadoContrato));
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "NombreCargo", empleadoContrato.IdCargo);
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato", empleadoContrato.IdTipoContrato);
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoContrato.IdEmpleado);
            return View(empleadoContrato);
        }

        // POST: EmpleadoContrato/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoContrato,IdEmpleado,CodigoContrato,IdTipoContrato,IdCargo,Estado,VigenciaMeses,FechaInicio,FechaFin,Salario,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoContrato empleadoContrato, string? numero)
        {
            try
            {
                if (id != empleadoContrato.IdEmpleadoContrato)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoContrato, false);


                    var consultaContratosActivos = await _context.EmpleadoContratos
                     .Where(c => c.IdEmpleado == empleadoContrato.IdEmpleado && c.Activo)
                     .ToListAsync();

                    // Verificar si hay contratos activos
                    if (consultaContratosActivos.Any() && empleadoContrato.Estado != EstadoContrato.Borrador)
                    {
                        TempData["Error"] = "No se puede actualizar porque ya que se encuentra un contrato activo.";

                        if (numero == "1")
                        {
                            return Redirect($"/Empleado/FichaEmpleado/{empleadoContrato.IdEmpleado}?tab=messages");
                        }
                        if (numero == "2")
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else if ((empleadoContrato.Estado == EstadoContrato.Finalizado || empleadoContrato.Estado == EstadoContrato.Cancelado || empleadoContrato.Estado == EstadoContrato.Borrador) && empleadoContrato.Activo)
                    {
                        TempData["Error"] = "No se puede activar un contrato en estado 'Borrador/Cancelado/Finalizado'.";

                        if (numero == "1")
                        {
                            return Redirect($"/Empleado/FichaEmpleado/{empleadoContrato.IdEmpleado}?tab=messages");
                        }
                        if (numero == "2")
                        {
                            return RedirectToAction("Index");
                        }
                    }

                    // Obtener el contrato existente
                    var existingContrato = await _context.EmpleadoContratos.FindAsync(id);
                    if (existingContrato == null)
                    {
                        return NotFound();
                    }

                    // Actualizar las propiedades del contrato existente con las nuevas
                    _context.Entry(existingContrato).CurrentValues.SetValues(empleadoContrato);

                    //_context.Update(empleadoContrato);
                    
                    await _context.SaveChangesAsync();
                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se actualizó exitosamente.";


                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoContrato.IdEmpleado}?tab=messages");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
                }

                ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "NombreCargo", empleadoContrato.IdCargo);
                ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato", empleadoContrato.IdTipoContrato);
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoContrato.IdEmpleado);

                // Agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar actualizar el registro. Por favor, verifique los datos.";

                return View(empleadoContrato);
            }
            catch (DbUpdateConcurrencyException)
            {

                // Agregar un mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar actualizar el registro. Por favor, intente nuevamente.";
                if (numero == "1")
                {
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoContrato.IdEmpleado}?tab=messages");
                }
                if (numero == "2")
                {
                    return RedirectToAction("Index");
                }
                return View(empleadoContrato);
            }
        }

        // GET: EmpleadoContrato/Delete/5
        public async Task<IActionResult> Delete(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoContrato = await _context.EmpleadoContratos
                .Include(e => e.IdCargoNavigation)
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoContrato == id);
            if (empleadoContrato == null)
            {
                return NotFound();
            }
            ViewBag.Numero = numero;
            return View(empleadoContrato);
        }

        // POST: EmpleadoContrato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? numero)
        {
            try
            {
                var empleadoContrato = await _context.EmpleadoContratos.FindAsync(id);

                if (empleadoContrato != null)
                {
                    _context.EmpleadoContratos.Remove(empleadoContrato);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se eliminó exitosamente.";

                   
                   

                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoContrato.IdEmpleado}?tab=messages");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
                }

                // Agregar mensaje de error a TempData
                TempData["Error"] = "No se encontró el registro que intenta eliminar. Por favor, verifique.";

                return Redirect($"/Empleado/FichaEmpleado/{empleadoContrato.IdEmpleado}?tab=messages");
            }
            catch (Exception ex)
            {
                // Puedes manejar la excepción de manera específica o simplemente registrarla
                // Logging.LogError(ex, "Error al intentar eliminar un registro");

                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar eliminar el registro. Por favor, intente nuevamente.";


                return View();
            }
        }

        private bool EmpleadoContratoExists(int id)
        {
            return _context.EmpleadoContratos.Any(e => e.IdEmpleadoContrato == id);
        }

        private void SetCamposAuditoria(EmpleadoContrato record, bool bNewRecord)
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
