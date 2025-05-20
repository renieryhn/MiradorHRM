using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PlanillaPM.Models;
using ClosedXML.Excel;
using static PlanillaPM.cGeneralFun;
using System.Data;

namespace PlanillaPM.Controllers
{
    public class EmpleadoEducacionController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoEducacionController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoEducacion
        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, int? estado)
        {
            IQueryable<EmpleadoEducacion> query = _context.EmpleadoEducacions;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.Institucion.ToLower().Contains(filter.ToLower()));
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

            List<EmpleadoEducacion> registros;
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

            //var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();

            return View(data);

        }

        [HttpGet]
        public ActionResult Download(int id, string filter)
        {
            // Aplicar los filtros necesarios
            var query = _context.EmpleadoEducacions
                                .Where(ec => ec.IdEmpleado == id);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(ec => ec.Institucion.ToLower().Contains(filter.ToLower()));
            }

            // Seleccionar los datos que se exportarán
            var data = query
                .Select(ec => new
                {
                    ec.IdEmpleadoEducacion,
                    ec.Institucion,
                    ec.TituloObtenido,
                    FechaDesde = ec.FechaDesde.ToString("dd/MM/yyyy"),
                    FechaHasta = ec.FechaHasta.ToString("dd/MM/yyyy"),
                    ec.Comentarios,
                    Activo = ec.Activo ? "Sí" : "No"
                })
                .ToList();

            // Convertir la lista a DataTable
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            string fileName = $"EmpleadoEducacion_{id}.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Educacion");
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


        [HttpGet]
        public ActionResult DownloadAll(string? filter, string? idEmpleado, int? estado)
        {
            IQueryable<EmpleadoEducacion> query = _context.EmpleadoEducacions
                .Include(ec => ec.IdEmpleadoNavigation);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.Institucion.ToLower().Contains(filter.ToLower()));
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
                .Select(ec => new
                {
                    ec.IdEmpleadoEducacion,
                    Empleado = ec.IdEmpleadoNavigation.NombreEmpleado + " " + ec.IdEmpleadoNavigation.ApellidoEmpleado,
                    ec.Institucion,
                    ec.TituloObtenido,
                    FechaDesde = ec.FechaDesde.ToString("dd/MM/yyyy"),
                    FechaHasta = ec.FechaHasta.ToString("dd/MM/yyyy"),
                    ec.Comentarios,
                    Activo = ec.Activo ? "Sí" : "No"
                })
                .ToList();

            if (!data.Any())
            {
                TempData["error"] = "No se encontraron registros con los filtros aplicados.";
                return RedirectToAction(nameof(Index), new { filter, idEmpleado, estado });
            }

            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            string fileName = "EmpleadoEducacion.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Educacion");
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



        // GET: EmpleadoEducacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoEducacion = await _context.EmpleadoEducacions
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoEducacion == id);
            if (empleadoEducacion == null)
            {
                return NotFound();
            }

            return View(empleadoEducacion);
        }

        // GET: EmpleadoEducacion/Create
        public IActionResult Create(int? id, int? idEmpleado)
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            return View();
        }

        // POST: EmpleadoEducacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoEducacion,IdEmpleado,Institucion,TituloObtenido,FechaDesde,FechaHasta,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoEducacion empleadoEducacion, int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoEducacion, true);
                    _context.Add(empleadoEducacion);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se creó exitosamente.";
                  
                   
                    if (id.HasValue)
                    {

                        if (id == 1)
                        {
                            return Redirect($"/Empleado/FichaEmpleado/{empleadoEducacion.IdEmpleado}?tab=settings");
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

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoEducacion.IdEmpleado);

                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, verifique los datos.";

                return View(empleadoEducacion);
            }
            catch (Exception ex)
            {
                
                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, intente nuevamente.";

                return View();
            }
        }

        // GET: EmpleadoEducacion/Edit/5
        public async Task<IActionResult> Edit(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoEducacion = await _context.EmpleadoEducacions.FindAsync(id);
            if (empleadoEducacion == null)
            {
                return NotFound();
            }
            ViewBag.Numero = numero;
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoEducacion.IdEmpleado);
            return View(empleadoEducacion);
        }

        // POST: EmpleadoEducacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoEducacion,IdEmpleado,Institucion,TituloObtenido,FechaDesde,FechaHasta,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoEducacion empleadoEducacion, string? numero)
        {
            try
            {
                if (id != empleadoEducacion.IdEmpleadoEducacion)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoEducacion, false);
                    _context.Update(empleadoEducacion);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se actualizó exitosamente.";
                                 

                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoEducacion.IdEmpleado}?tab=settings");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
                }

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoEducacion.IdEmpleado);

                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar actualizar el registro. Por favor, verifique los datos.";

                return View(empleadoEducacion);
            }
            catch (DbUpdateConcurrencyException)
            {
               
                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar actualizar el registro. Por favor, intente nuevamente.";
                return View();
            }
        }

        // GET: EmpleadoEducacion/Delete/5
        public async Task<IActionResult> Delete(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoEducacion = await _context.EmpleadoEducacions
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoEducacion == id);
            if (empleadoEducacion == null)
            {
                return NotFound();
            }
            ViewBag.Numero = numero;
            return View(empleadoEducacion);
        }

        // POST: EmpleadoEducacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? numero)
        {
            try
            {
                var empleadoEducacion = await _context.EmpleadoEducacions.FindAsync(id);
                if (empleadoEducacion != null)
                {
                    _context.EmpleadoEducacions.Remove(empleadoEducacion);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se eliminó exitosamente.";
                }
             
                
                if (numero == "1")
                {
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoEducacion.IdEmpleado}?tab=settings");
                }
                if (numero == "2")
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar eliminar el registro. Por favor, intente nuevamente.";

                return View();
            }
            return View();
        }

        private bool EmpleadoEducacionExists(int id)
        {
            return _context.EmpleadoEducacions.Any(e => e.IdEmpleadoEducacion == id);
        }
        private void SetCamposAuditoria(EmpleadoEducacion record, bool bNewRecord)
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
