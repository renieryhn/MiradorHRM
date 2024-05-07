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
using PlanillaPM.ViewModel;

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
        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, int? estado)
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

            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentIdEmpleado = idEmpleado;
            ViewBag.CurrentEstado = estado; 

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

       

        public ActionResult Download(int id)
        {
            // Filtrar los contactos de empleado por el id recibido
            List<EmpleadoContacto> data = _context.EmpleadoContactos.Where(ec => ec.IdEmpleado == id).ToList();

            // Convertir la lista de contactos en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = $"EmpleadoContacto{id}.xlsx";

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


        public async Task<IActionResult> FichaEmpleado(int? id)
        {
            var Empleados = await _context.Empleados.ToListAsync();           

            if (id == null)
            {
                return NotFound();
            }

            var EmpleadoSeleccionado = await _context.Empleados
           .Include(e => e.IdBancoNavigation)
           .Include(e => e.IdCargoNavigation)
           .Include(e => e.IdDepartamentoNavigation)
           .Include(e => e.IdEncargadoNavigation)
           .Include(e => e.IdTipoContratoNavigation)
           .Include(e => e.IdTipoNominaNavigation)
           .Include(e => e.EmpleadoContactos)
           .Include(e => e.EmpleadoContratos)
           .Include(e => e.EmpleadoEducacions)
           .Include(e => e.EmpleadoExperiencia)
           .Include(e => e.EmpleadoHabilidads)
           .Include(e => e.EmpleadoAusencia)
           .Include(e => e.EmpleadoActivos)
           .FirstOrDefaultAsync(m => m.IdEmpleado == id);

            if (EmpleadoSeleccionado == null)
            {
                return NotFound();
            }

            //if (EmpleadoSeleccionado.Fotografia != null)
            //{
            //    var base64Image = Convert.ToBase64String(EmpleadoSeleccionado.Fotografia);
            //    EmpleadoSeleccionado.FotografiaBase64 = "data:image/jpeg;base64," + base64Image;
            //}
            //else
            //{
            //    EmpleadoSeleccionado.FotografiaBase64 = Url.Content("~/img/Employee.png");
            //}

            if (EmpleadoSeleccionado.Fotografia != null)
            {
                var nombreArchivo = EmpleadoSeleccionado.FotografiaName;
            }
            else
            {
                // emple.FotografiaBase64 = "img/Employee.png";
                EmpleadoSeleccionado.FotografiaName = Url.Content("~/EmpleadoImg/Employee.png");
            }

            var viewModel = new EmpleadoViewModel
            {
                EmpleadoSeleccionado = EmpleadoSeleccionado,
                Empleados = Empleados,
                
            };

            var nombreEmpleado = EmpleadoSeleccionado.NombreCompleto;
            ViewBag.NombreEmpleado = nombreEmpleado;
            ViewBag.IdEmpleado = EmpleadoSeleccionado.IdEmpleado;
            return View(viewModel);
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
        public async Task<IActionResult> Create([Bind("IdContactoEmergencia,IdEmpleado,NombreContacto,Relacion,Celular,TelefonoFijo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoContacto empleadoContacto, int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetCamposAuditoria(empleadoContacto, true);
                    _context.Add(empleadoContacto);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Se ha agregado un nuevo Empleado Contacto exitosamente";
                   
                   
                    if (id.HasValue)
                    {

                        if (id == 1)
                        {
                            return Redirect($"/Empleado/FichaEmpleado/{empleadoContacto.IdEmpleado}?tab=profile");
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

                TempData["Error"] = "Hubo un error al intentar crear el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
            }

            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoContacto.IdEmpleado);
            return View(empleadoContacto);
        }

        // GET: EmpleadoContacto/Edit/5
        public async Task<IActionResult> Edit(int? id, string? numero)
        {

            if (id == null)
            {
                return NotFound();
            }
           ;
            var empleadoContacto = await _context.EmpleadoContactos.FindAsync(id);

            if (empleadoContacto == null)
            {
                return NotFound();
            }
            ViewBag.Numero = numero;
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleadoContacto.IdEmpleado);
            return View(empleadoContacto);
        }

        // POST: EmpleadoContacto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdContactoEmergencia,IdEmpleado,NombreContacto,Relacion,Celular,TelefonoFijo,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] EmpleadoContacto empleadoContacto, string? numero)
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

                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoContacto.IdEmpleado}?tab=profile");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
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
                return View();
            }
        }

        // GET: EmpleadoContacto/Delete/5
        public async Task<IActionResult> Delete(int? id, string? numero)
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

            ViewBag.Numero = numero;
            return View(empleadoContacto);
        }

        // POST: EmpleadoContacto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? numero)
        {
           
            try
            {
                var empleadoContacto = await _context.EmpleadoContactos.FindAsync(id);
                if (empleadoContacto != null)
                {
                    _context.EmpleadoContactos.Remove(empleadoContacto);
                    await _context.SaveChangesAsync();


                    TempData["success"] = "Empleado Contacto eliminado exitosamente.";
                   
                    if (numero == "1")
                    {
                        return Redirect($"/Empleado/FichaEmpleado/{empleadoContacto.IdEmpleado}?tab=profile");
                    }
                    if (numero == "2")
                    {
                        return RedirectToAction("Index");
                    }
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


            return View();
           
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
