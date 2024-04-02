using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using Microsoft.AspNetCore.Identity;
using ClosedXML.Excel;
using static PlanillaPM.cGeneralFun;
using System.Data;

namespace PlanillaPM.Controllers
{
    public class EmpleadoContactoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoContactoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoContacto
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            //var planillaContext = _context.EmpleadoContactos.Include(e => e.IdEmpleadoNavigation);
            //return View(await planillaContext.ToListAsync());

            List<EmpleadoContacto> registros;
            if (filter != null)
            {
                registros = await _context.EmpleadoContactos.Where(r => r.NombreContacto.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.EmpleadoContactos.ToListAsync();
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

        public ActionResult Download()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            List<EmpleadoContacto>? data = null;
            if (data == null)
            {
                data = _context.EmpleadoContactos.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "EmpleadoContacto.xlsx";
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

        // GET: EmpleadoContacto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoContacto = await _context.EmpleadoContactos
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdContactoEmergencia == id);
            if (empleadoContacto == null)
            {
                return NotFound();
            }

            return View(empleadoContacto);
        }

        // GET: EmpleadoContacto/Create
        public IActionResult Create()
        {

            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            return View();
        }

        // POST: EmpleadoContacto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdContactoEmergencia,IdEmpleado,NombreContacto,Relacion,Celular,TelefonoFijo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoContacto empleadoContacto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoContacto, true);
                    _context.Add(empleadoContacto);
                    await _context.SaveChangesAsync();

                    TempData["mensaje"] = "Se ha agregado un nuevo Empleado Contacto exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {

                TempData["Error"] = "Hubo un error al intentar crear el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
            }

            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoContacto.IdEmpleado);
            return View(empleadoContacto);
        }

        // GET: EmpleadoContacto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoContacto = await _context.EmpleadoContactos.FindAsync(id);
            if (empleadoContacto == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoContacto.IdEmpleado);
            return View(empleadoContacto);
        }

        // POST: EmpleadoContacto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdContactoEmergencia,IdEmpleado,NombreContacto,Relacion,Celular,TelefonoFijo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoContacto empleadoContacto)
        {
            if (id != empleadoContacto.IdContactoEmergencia)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoContacto, false);
                    _context.Update(empleadoContacto);
                    await _context.SaveChangesAsync();

                    TempData["mensaje"] = "Empleado Contacto actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoContacto.IdEmpleado);
                TempData["Error"] = "Hubo un error al intentar actualizar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                return View(empleadoContacto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoContactoExists(empleadoContacto.IdContactoEmergencia))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un error durante la operación de actualización. Por favor, intenta nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: EmpleadoContacto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoContacto = await _context.EmpleadoContactos
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdContactoEmergencia == id);
            if (empleadoContacto == null)
            {
                return NotFound();
            }

            return View(empleadoContacto);
        }

        // POST: EmpleadoContacto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var empleadoContacto = await _context.EmpleadoContactos.FindAsync(id);
                if (empleadoContacto != null)
                {
                    _context.EmpleadoContactos.Remove(empleadoContacto);
                    await _context.SaveChangesAsync();


                    TempData["mensaje"] = "Empleado Contacto eliminado exitosamente.";
                }
                else
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                }
            }        
            catch (Exception ex)
            {
                // Manejar la excepción según tus necesidades, puedes registrarla, mostrar un mensaje específico, etc.
                TempData["Error"] = "Hubo un error durante la operación de eliminación. Por favor, intenta nuevamente.";
            }


            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoContactoExists(int id)
        {
            return _context.EmpleadoContactos.Any(e => e.IdContactoEmergencia == id);
        }
        private void SetCamposAuditoria(EmpleadoContacto record, bool bNewRecord)
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
