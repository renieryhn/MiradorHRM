using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using static PlanillaPM.cGeneralFun;

namespace PlanillaPM.Controllers
{
    public class EmpleadoHabilidadController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoHabilidadController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoHabilidad
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            //var planillaContext = _context.EmpleadoHabilidads.Include(e => e.IdEmpleadoNavigation);
            //return View(await planillaContext.ToListAsync());

            List<EmpleadoHabilidad> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoHabilidads.Where(r => r.Habilidad.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoHabilidads.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
            return View(data);
        }

        public ActionResult Download(int id)
        {
            // Filtrar los contactos de empleado por el id recibido
            List<EmpleadoHabilidad> data = _context.EmpleadoHabilidads.Where(ec => ec.IdEmpleado == id).ToList();

            // Convertir la lista de contactos en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = $"EmpleadoHabilidad_{id}.xlsx";

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

        public ActionResult DownloadAll()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            List<EmpleadoHabilidad>? data = null;
            if (data == null)
            {
                data = _context.EmpleadoHabilidads.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "EmpleadoHabilidad.xlsx";
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

        // GET: EmpleadoHabilidad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHabilidad = await _context.EmpleadoHabilidads
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoHabilidad == id);
            if (empleadoHabilidad == null)
            {
                return NotFound();
            }

            return View(empleadoHabilidad);
        }

        // GET: EmpleadoHabilidad/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            return View();
        }

        // POST: EmpleadoHabilidad/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoHabilidad,IdEmpleado,Habilidad,ExperienciaYears,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoHabilidad empleadoHabilidad, int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoHabilidad, true);
                    _context.Add(empleadoHabilidad);
                    await _context.SaveChangesAsync();

                    // Mensaje de éxito
                    TempData["success"] = "La habilidad del empleado se creó exitosamente.";
                    
                    if (id.HasValue)
                    {
                        if (id == 1)
                        {
                            return Redirect($"/Empleado/FichaEmpleado/{empleadoHabilidad.IdEmpleado}?tab=Habilidad");
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
            }
            catch (Exception ex)
            {

                // Mensaje de error
                TempData["Error"] = "Ha ocurrido un error al intentar guardar la información. Por favor, inténtelo de nuevo.";
            }

            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoHabilidad.IdEmpleado);
            return View(empleadoHabilidad);
        }

        // GET: EmpleadoHabilidad/Edit/5
        public async Task<IActionResult> Edit(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHabilidad = await _context.EmpleadoHabilidads.FindAsync(id);
            if (empleadoHabilidad == null)
            {
                return NotFound();
            }
            ViewBag.Numero = numero;
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoHabilidad.IdEmpleado);
            return View(empleadoHabilidad);
        }

        // POST: EmpleadoHabilidad/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoHabilidad,IdEmpleado,Habilidad,ExperienciaYears,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoHabilidad empleadoHabilidad, string? numero)
        {
            try
            {
                if (id != empleadoHabilidad.IdEmpleadoHabilidad)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoHabilidad, false);
                    _context.Update(empleadoHabilidad);
                    await _context.SaveChangesAsync();

                    // Mensaje de éxito
                    TempData["success"] = "La habilidad del empleado se ha actualizado exitosamente.";
                    

                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoHabilidad.IdEmpleado}?tab=Habilidad");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoHabilidadExists(empleadoHabilidad.IdEmpleadoHabilidad))
                {
                    return NotFound();
                }
                else
                {

                    // Mensaje de error
                    TempData["Error"] = "Ha ocurrido un error al intentar actualizar la información. Por favor, inténtelo de nuevo.";
                }
            }

            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoHabilidad.IdEmpleado);
            return View(empleadoHabilidad);
        }

        // GET: EmpleadoHabilidad/Delete/5
        public async Task<IActionResult> Delete(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHabilidad = await _context.EmpleadoHabilidads
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoHabilidad == id);
            if (empleadoHabilidad == null)
            {
                return NotFound();
            }
            ViewBag.Numero = numero;
            return View(empleadoHabilidad);
        }

        // POST: EmpleadoHabilidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? numero)
        {
            try
            {
                var empleadoHabilidad = await _context.EmpleadoHabilidads.FindAsync(id);
                if (empleadoHabilidad != null)
                {
                    _context.EmpleadoHabilidads.Remove(empleadoHabilidad);
                    await _context.SaveChangesAsync();

                    // Mensaje de éxito
                    TempData["success"] = "La habilidad del empleado se ha eliminado exitosamente.";
                    
                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoHabilidad.IdEmpleado}?tab=Habilidad");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {

                // Mensaje de error
                TempData["Error"] = "Ha ocurrido un error al intentar eliminar la habilidad del empleado. Por favor, inténtelo de nuevo.";
            }

            return View();
        }

        private bool EmpleadoHabilidadExists(int id)
        {
            return _context.EmpleadoHabilidads.Any(e => e.IdEmpleadoHabilidad == id);
        }

        private void SetCamposAuditoria(EmpleadoHabilidad record, bool bNewRecord)
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
