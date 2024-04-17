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
        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado)
        {
            IQueryable<EmpleadoContacto> query = _context.EmpleadoContactos;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.NombreContacto.ToLower().Contains(filter.ToLower()));
            }
            if (!String.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdEmpleado.ToString().Contains(idEmpleado));
            }

            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentIdEmpleado = idEmpleado;

            List<EmpleadoContacto> registros;
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
            } else
            {
                ViewData["IdEmpleado"] = new SelectList(IdEmpleadoNavigation, "IdEmpleado", "NombreCompleto");
            }

           
            return View(data);


        }


        public async Task<IActionResult> Filtros()
        {
            try
            {
                // Obtener todos los empleados de la base de datos
                var empleados = await _context.Empleados.ToListAsync();

                // Verificar si se encontraron empleados
                if (empleados != null && empleados.Any())
                {
                    // Pasar la lista de empleados a la vista usando ViewBag
                    ViewBag.Empleados = empleados;

                    // O si prefieres, pasar la lista de empleados al modelo
                    // TuModelo.Empleados = empleados;

                    return View();
                }
                else
                {
                    
                    return RedirectToAction("Error");
                }
            }
            catch (Exception ex)
            {           
                return RedirectToAction("Error");
            }
        }

        public ActionResult Download(int id)
        {
            // Filtrar los contactos de empleado por el id recibido
            List<EmpleadoContacto> data = _context.EmpleadoContactos.Where(ec => ec.IdEmpleado == id).ToList();

            // Convertir la lista de contactos en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = $"EmpleadoContacto_{id}.xlsx";

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

                    TempData["success"] = "Se ha agregado un nuevo Empleado Contacto exitosamente";
                   
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoContacto.IdEmpleado}?tab=profile");


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
        public async Task<IActionResult> Edit(int? id, string returnUrl)
        {

            if (id == null)
            {
                return NotFound();
            }
            ViewBag.ReturnUrl = returnUrl;
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
        public async Task<IActionResult> Edit(int id, [Bind("IdContactoEmergencia,IdEmpleado,NombreContacto,Relacion,Celular,TelefonoFijo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoContacto empleadoContacto, string returnUrl)
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

                    TempData["success"] = "Empleado Contacto actualizado exitosamente.";
                    //volver a la pagina que invocó el edit 

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        //return RedirectToAction(nameof(Index));
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoContacto.IdEmpleado}?tab=profile");
                    }
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


                    TempData["success"] = "Empleado Contacto eliminado exitosamente.";
                    return Redirect($"/Empleado/FichaEmpleado/{empleadoContacto.IdEmpleado}?tab=profile");
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
