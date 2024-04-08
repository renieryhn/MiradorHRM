using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using static PlanillaPM.cGeneralFun;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using DocumentFormat.OpenXml.Vml;
using Microsoft.Win32;
using static PlanillaPM.Models.EmpleadoActivo;
using static PlanillaPM.Models.Empleado;

namespace PlanillaPM.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly PlanillaContext _context;

        public EmpleadoController(PlanillaContext context)
        {
            _context = context;
        }

        // GET: Empleado
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            List<Empleado> registros;

            if (filter != null)
            {
                registros = await _context.Empleados.Where(r => r.NombreEmpleado.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Empleados.ToListAsync();
            }

            const int pageSize = 9;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;


            foreach (var empleado in registros)
            {
                if (empleado.Fotografia != null)
                {
                    var base64Image = Convert.ToBase64String(empleado.Fotografia);
                    empleado.FotografiaBase64 = "data:image/jpeg;base64," + base64Image;
                }
                else
                {
                    empleado.FotografiaBase64 = "img/Employee.png";
                }
            }
            var IdDepartamentoNavigation = await _context.Departamentos.ToListAsync();
            var IdCargoNavigation = await _context.Cargos.ToListAsync();
            var IdEncargadoNavigation = await _context.Empleados.ToListAsync();
            var IdBancoNavigation = await _context.Bancos.ToListAsync();
            var IdTipoContratoNavigation = await _context.TipoContratos.ToListAsync();
            var IdTipoNominaNavigation = await _context.TipoNominas.ToListAsync();

            return View(data);
        }
        public IActionResult ObtenerMenuDinamico()
        {
            var gen = new cGeneralFun();
            var menu = gen.ObtenerMenu("Empleado"); // Obtener el menú para el perfil
            return PartialView("_MenuDinamico", menu); // Retornar el partial con la lista de menús
        }
        //Action result que devuelve el id y nombre completo del empleado para llenar el dropdownlist
        public JsonResult GetEmpleado()
        {
            var empleados = _context.Empleados.Where(e => e.Activo).Select(e => new { e.IdEmpleado, e.NombreCompleto }).ToList();
            return Json(empleados);
        }

        public ActionResult Download()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            List<Empleado>? data = null;

            if (data == null)
            {
                data = _context.Empleados.ToList();
            }
            DataTable table = converter.ToDataTable(data);

            string fileName = "Empleados.xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                //Add DataTable in worksheet  
                wb.Worksheets.Add(table);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //Return xlsx Excel File  
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        // GET: Empleado/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var emple = await _context.Empleados.FindAsync(id);
            var emple = await _context.Empleados.Include(e => e.EmpleadoContactos).Where(e => e.IdEmpleado == id).FirstOrDefaultAsync();
            if (emple == null)
            {
                return NotFound();
            }

            if (emple.Fotografia != null)
            {
                var base64Image = Convert.ToBase64String(emple.Fotografia);
                emple.FotografiaBase64 = "data:image/jpeg;base64," + base64Image;
            }
            else
            {
                // emple.FotografiaBase64 = "img/Employee.png";
                emple.FotografiaBase64 = Url.Content("~/img/Employee.png");
            }

            var empleado = await _context.Empleados
                .Include(e => e.IdBancoNavigation)
                .Include(e => e.IdCargoNavigation)
                .Include(e => e.IdDepartamentoNavigation)
                .Include(e => e.IdEncargadoNavigation)
                .Include(e => e.IdTipoContratoNavigation)
                .Include(e => e.IdTipoNominaNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: Empleado/Create
        public IActionResult Create()
        {
            ViewBag.EstadoCivilEmpleado = Enum.GetValues(typeof(EstadoCivilEmpleado));
            ViewData["IdBanco"] = new SelectList(_context.Bancos.Where(r => r.Activo), "IdBanco", "NombreBanco");
            ViewData["IdCargo"] = new SelectList(_context.Cargos.Where(r => r.Activo), "IdCargo", "NombreCargo");
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos.Where(r => r.Activo), "IdDepartamento", "NombreDepartamento");
            ViewData["IdEncargado"] = new SelectList(_context.Empleados.Where(r => r.Activo), "IdEmpleado", "NombreCompleto");
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos.Where(r => r.Activo), "IdTipoContrato", "NombreTipoContrato");
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas.Where(r => r.Activo), "IdTipoNomina", "NombreTipoNomina");
            return View();
        }

        // POST: Empleado/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleado,CodigoInterno,NombreEmpleado,ApellidoEmpleado,NumeroIdentidad,NumeroLicencia,FechaVencimientoLicencia,Nacionalidad,FechaNacimiento,Genero,Direccion,Telefono,CiudadResidencia,Email,Activo,IdCargo,IdDepartamento,IdTipoContrato,IdTipoNomina,IdEncargado,EstadoCivil,FechaInicio,IdBanco,CuentaBancaria,NumeroRegistroTributario,SalarioBase")] Empleado empleado, IFormFile FotoTmp)
        {

            try
            {
                empleado.FechaCreacion = DateTime.Now;
                empleado.CreadoPor = "Admin";
                empleado.FechaModificacion = DateTime.Now;
                empleado.ModificadoPor = "Admin";

                // Verificar si el IdBanco seleccionado es "Seleccionar" y establecerlo como null
                if (empleado.IdBanco == 0)
                {
                    empleado.IdBanco = null;
                }

                if (ModelState.IsValid)
                {
                    if (FotoTmp != null && FotoTmp.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await FotoTmp.CopyToAsync(memoryStream);
                            empleado.Fotografia = memoryStream.ToArray();
                        }
                    }
                    _context.Add(empleado);
                    await _context.SaveChangesAsync();
                    TempData["mensaje"] = "Empleado creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                   // var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                  //  TempData["Error"] = "Error: " + message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, intente nuevamente.";
               // TempData["error"] = "Error: " + ex.Message;
            }


            ViewData["IdBanco"] = new SelectList(_context.Bancos.Where(r => r.Activo), "IdBanco", "NombreBanco");
            ViewData["IdCargo"] = new SelectList(_context.Cargos.Where(r => r.Activo), "IdCargo", "NombreCargo");
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos.Where(r => r.Activo), "IdDepartamento", "NombreDepartamento");
            ViewData["IdEncargado"] = new SelectList(_context.Empleados.Where(r => r.Activo), "IdEmpleado", "NombreCompleto");
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos.Where(r => r.Activo), "IdTipoContrato", "NombreTipoContrato");
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas.Where(r => r.Activo), "IdTipoNomina", "NombreTipoNomina");
            return View(empleado);
        }

        // GET: Empleado/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado == null)
            {
                return NotFound();
            }

            if (empleado.Fotografia != null)
            {
                var base64Image = Convert.ToBase64String(empleado.Fotografia);
                empleado.FotografiaBase64 = "data:image/jpeg;base64," + base64Image;
            }
            else
            {
                // emple.FotografiaBase64 = "img/Employee.png";
                empleado.FotografiaBase64 = Url.Content("~/img/Employee.png");
            }

            ViewBag.EstadoCivilEmpleado = Enum.GetValues(typeof(Empleado.EstadoCivilEmpleado));
            ViewData["IdBanco"] = new SelectList(_context.Bancos.Where(r => r.Activo), "IdBanco", "NombreBanco", empleado.IdBanco);
            ViewData["IdCargo"] = new SelectList(_context.Cargos.Where(r => r.Activo), "IdCargo", "NombreCargo", empleado.IdCargo);
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos.Where(r => r.Activo), "IdDepartamento", "NombreDepartamento", empleado.IdDepartamento);
            ViewData["IdEncargado"] = new SelectList(_context.Empleados.Where(r => r.Activo), "IdEmpleado", "NombreEmpleado", empleado.IdEncargado);
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos.Where(r => r.Activo), "IdTipoContrato", "NombreTipoContrato", empleado.IdTipoContrato);
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas.Where(r => r.Activo), "IdTipoNomina", "NombreTipoNomina", empleado.IdTipoNomina);
            return View(empleado);
        }

        // POST: Empleado/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleado,CodigoInterno,NombreEmpleado,ApellidoEmpleado,NumeroIdentidad,NumeroLicencia,FechaVencimientoLicencia,Nacionalidad,FechaNacimiento,Genero,Fotografia,Direccion,Telefono,CiudadResidencia,Email,Activo,IdCargo,IdDepartamento,IdTipoContrato,IdTipoNomina,IdEncargado,EstadoCivil,FechaInicio,IdBanco,CuentaBancaria,NumeroRegistroTributario,SalarioBase,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Empleado empleado)
        {
            try
            {
                if (id != empleado.IdEmpleado)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    _context.Update(empleado);
                    await _context.SaveChangesAsync();
                    TempData["mensaje"] = "Empleado actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoExists(empleado.IdEmpleado))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            // Si llegamos a este punto, significa que hubo un error de validación
            TempData["Error"] = "Error: Por favor, corrija los errores e intente nuevamente.";

            ViewData["IdBanco"] = new SelectList(_context.Bancos, "IdBanco", "NombreBanco", empleado.IdBanco);
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "NombreCargo", empleado.IdCargo);
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "IdDepartamento", "NombreDepartamento", empleado.IdDepartamento);
            ViewData["IdEncargado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreEmpleado", empleado.IdEncargado);
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato", empleado.IdTipoContrato);
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas, "IdTipoNomina", "NombreTipoNomina", empleado.IdTipoNomina);
            return View(empleado);
        }

        // GET: Empleado/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var emple = await _context.Empleados.FindAsync(id);

            if (emple == null)
            {
                return NotFound();
            }

            if (emple.Fotografia != null)
            {
                var base64Image = Convert.ToBase64String(emple.Fotografia);
                emple.FotografiaBase64 = "data:image/jpeg;base64," + base64Image;
            }
            else
            {
                // emple.FotografiaBase64 = "img/Employee.png";
                emple.FotografiaBase64 = Url.Content("~/img/Employee.png");
            }

            var empleado = await _context.Empleados
                .Include(e => e.IdBancoNavigation)
                .Include(e => e.IdCargoNavigation)
                .Include(e => e.IdDepartamentoNavigation)
                .Include(e => e.IdEncargadoNavigation)
                .Include(e => e.IdTipoContratoNavigation)
                .Include(e => e.IdTipoNominaNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            try
            {
                _context.Empleados.Remove(empleado);
                await _context.SaveChangesAsync();
                TempData["success"] = "Empleado eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_"))
                {
                    TempData["Error"] = "Error: No puede elimiar el registro actual ya que se encuentra relacionado a otro Registro.";
                }
                else
                {
                    var message = ex.InnerException;
                    TempData["error"] = "Error: " + message;
                }
                return View(empleado);
            }

        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.IdEmpleado == id);
        }
        [HttpGet]
        public IActionResult RenderPartialEmpleado(int id)
        {
            var listaDeEmpleados = _context.Empleados.Where(e => e.IdEmpleado == id).ToList();
            return PartialView("_Empleado", listaDeEmpleados);
        }
    }

}
