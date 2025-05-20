using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using Microsoft.AspNetCore.Identity;
using static PlanillaPM.cGeneralFun;

namespace PlanillaPM.Controllers
{
    public class EmpleadoExperienciumController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoExperienciumController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoExperiencium
        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, int? estado)
        {


            IQueryable<EmpleadoExperiencium> query = _context.EmpleadoExperiencia;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.Empresa.ToLower().Contains(filter.ToLower()));
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

            List<EmpleadoExperiencium> registros;
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
        public ActionResult Download(int id, string? filter)
        {
            var query = _context.EmpleadoExperiencia
                .Include(ec => ec.IdEmpleadoNavigation)
                .Where(ec => ec.IdEmpleado == id);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(ec => ec.Empresa.ToLower().Contains(filter.ToLower()));
            }
           
            var data = query.Select(ec => new
            {
                ec.IdEmpleadoExperiencia,
                Empleado = ec.IdEmpleadoNavigation.NombreEmpleado + " " + ec.IdEmpleadoNavigation.ApellidoEmpleado,
                ec.Empresa,
                ec.Cargo,
                FechaDesde = ec.FechaDesde.ToString("dd/MM/yyyy"),
                FechaHasta = ec.FechaHasta.ToString("dd/MM/yyyy"),
                Descripcion = ec.Descripcion ?? "N/A",
                Activo = ec.Activo ? "Sí" : "No"
            }).ToList();

            // Convertir a DataTable
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            string fileName = $"EmpleadoExperiencia_{id}.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Experiencia");
                worksheet.Columns().AdjustToContents();

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
            IQueryable<EmpleadoExperiencium> query = _context.EmpleadoExperiencia
                .Include(ec => ec.IdEmpleadoNavigation);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.Empresa.ToLower().Contains(filter.ToLower()));
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
                    ec.IdEmpleadoExperiencia,
                    Empleado = ec.IdEmpleadoNavigation.NombreEmpleado + " " + ec.IdEmpleadoNavigation.ApellidoEmpleado,
                    ec.Empresa,
                    ec.Cargo,
                    FechaDesde = ec.FechaDesde.ToString("dd/MM/yyyy"),
                    FechaHasta = ec.FechaHasta.ToString("dd/MM/yyyy"),
                    Descripcion = ec.Descripcion ?? "N/A",
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

            string fileName = "EmpleadoExperiencia.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Experiencia");
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




        // GET: EmpleadoExperiencium/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoExperiencium = await _context.EmpleadoExperiencia
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoExperiencia == id);
            if (empleadoExperiencium == null)
            {
                return NotFound();
            }

            return View(empleadoExperiencium);
        }

        // GET: EmpleadoExperiencium/Create
        public IActionResult Create(int? id, int? idEmpleado)
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            return View();
        }

        // POST: EmpleadoExperiencium/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleadoExperiencia,IdEmpleado,Empresa,Cargo,FechaDesde,FechaHasta,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoExperiencium empleadoExperiencium, int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoExperiencium, true);
                    _context.Add(empleadoExperiencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se creó exitosamente.";

                    if (id.HasValue)
                    {
                        if (id == 1)
                        {
                            return Redirect($"/Empleado/FichaEmpleado/{empleadoExperiencium.IdEmpleado}?tab=Experiencium");
                        }
                        if (id == 2)
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["error"] = "Error no se encontró el valor de la dirección";
                        return RedirectToAction("Index");
                    }
                }

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoExperiencium.IdEmpleado);
                return View(empleadoExperiencium);
            }
            catch (Exception ex)
            {

                TempData["Error"] = ex.ToString();
                // Agregar mensaje de error a TempData
                //TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, intente nuevamente.";

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoExperiencium.IdEmpleado);
                return View(empleadoExperiencium);
            }
        }

        // GET: EmpleadoExperiencium/Edit/5
        public async Task<IActionResult> Edit(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoExperiencium = await _context.EmpleadoExperiencia.FindAsync(id);
            if (empleadoExperiencium == null)
            {
                return NotFound();
            }
            ViewBag.Numero = numero;
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoExperiencium.IdEmpleado);
            return View(empleadoExperiencium);
        }

        // POST: EmpleadoExperiencium/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoExperiencia,IdEmpleado,Empresa,Cargo,FechaDesde,FechaHasta,Descripcion,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoExperiencium empleadoExperiencium, string? numero)
        {
            try
            {
                if (id != empleadoExperiencium.IdEmpleadoExperiencia)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoExperiencium, false);
                    _context.Update(empleadoExperiencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se actualizó exitosamente.";

                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoExperiencium.IdEmpleado}?tab=Experiencium");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
                }
                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoExperiencium.IdEmpleado);
                return View(empleadoExperiencium);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Agregar mensaje de error a TempData
                TempData["Error"] = "Hubo un problema al intentar editar el registro. Por favor, intente nuevamente.";

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoExperiencium.IdEmpleado);
                return View(empleadoExperiencium);
            }
        }

        // GET: EmpleadoExperiencium/Delete/5
        public async Task<IActionResult> Delete(int? id, string? numero)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoExperiencium = await _context.EmpleadoExperiencia
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleadoExperiencia == id);
            if (empleadoExperiencium == null)
            {
                return NotFound();
            }

            ViewBag.Numero = numero;
            return View(empleadoExperiencium);
        }

        // POST: EmpleadoExperiencium/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? numero)
        {
            try
            {
                var empleadoExperiencium = await _context.EmpleadoExperiencia.FindAsync(id);
                if (empleadoExperiencium != null)
                {
                    _context.EmpleadoExperiencia.Remove(empleadoExperiencium);
                    await _context.SaveChangesAsync();

                    // Agregar mensaje de éxito a TempData
                    TempData["success"] = "El registro se eliminó exitosamente.";
                }
             
                if (numero == "1")
                {
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoExperiencium.IdEmpleado}?tab=Experiencium");
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

        private bool EmpleadoExperienciumExists(int id)
        {
            return _context.EmpleadoExperiencia.Any(e => e.IdEmpleadoExperiencia == id);
        }

        private void SetCamposAuditoria(EmpleadoExperiencium record, bool bNewRecord)
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
