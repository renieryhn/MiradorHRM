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
using Syncfusion.Pdf;
using Syncfusion.DocIORenderer;
using Syncfusion.HtmlConverter;
using DocumentFormat.OpenXml.Office2010.Excel;
using PlanillaPM.ViewModel;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using PdfSharp.Pdf.Filters;

namespace PlanillaPM.Controllers
{
    public class NominaEmpleadoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Usuario> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IWebHostEnvironment Environment;

        public NominaEmpleadoController(PlanillaContext context, Microsoft.AspNetCore.Identity.UserManager<Usuario> userManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Environment = environment;
        }
        //Action result que devuelve el id y nombre completo del empleado para llenar el dropdownlist
        public JsonResult GetEmpleado()
        {
            var empleados = _context.Empleados.Where(e => e.Activo).Select(e => new { e.IdEmpleado, e.NombreCompleto }).ToList();
            return Json(empleados);
        }

        public async Task<IActionResult> Index(int pg, string? filter, string? idEmpleado, string? idTipoNomina, int? estado)
        {
            IQueryable<Empleado> query = _context.Empleados;

            if (!String.IsNullOrEmpty(filter))
            {
                query = query.Where(r => r.NombreCompleto.ToLower().Contains(filter.ToLower()));
            }
            if (!String.IsNullOrEmpty(idEmpleado))
            {
                query = query.Where(r => r.IdEmpleado.ToString().Contains(idEmpleado));
            }   
            if (!String.IsNullOrEmpty(idTipoNomina))
            {
                query = query.Where(r => r.IdTipoNomina.ToString().Contains(idTipoNomina));
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
            ViewBag.CurrentIdTipoNomina = idTipoNomina;
            ViewBag.CurrentEstado = estado;

            //List<Empleado> registros;
            //registros = await query.Include(e => e.IdEmpleadoNavigation).ToListAsync();

            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = query.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = query.Skip(recSkip).Take(pager.PageSize).ToList();
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
            var IdTipoNomina = await _context.TipoNominas.ToListAsync();
            var IdDepartamentoNavigation = await _context.Departamentos.ToListAsync();
            var IdCargoNavigation = await _context.Cargos.ToListAsync();
            var IdBancoNavigation = await _context.Bancos.ToListAsync();
            var IdTipoContratoNavigation = await _context.TipoContratos.ToListAsync();
            var IdTipoNominaNavigation = await _context.TipoNominas.ToListAsync();
            ViewBag.IdTipoNomina = new SelectList(IdTipoNomina, "IdTipoNomina", "NombreTipoNomina");


            return View(data);

        }
        public IActionResult ObtenerMenuDinamico()
        {
            var gen = new cGeneralFun();
            var menu = gen.ObtenerMenu("NominaEmpleado"); // Obtener el menú para el perfil
            return PartialView("_MenuDinamico", menu); // Retornar el partial con la lista de menús
        }
        public async Task<IActionResult> IDIEmpleado(int? id, bool? estado)
        {
            var Empleados = await _context.Empleados.ToListAsync();
            var empleadosActivos = await _context.Empleados.Where(e => e.Activo == true).ToListAsync();

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
           .FirstOrDefaultAsync(m => m.IdEmpleado == id);

            if (EmpleadoSeleccionado == null)
            {
                return NotFound();
            }


            if (EmpleadoSeleccionado.FotografiaName != null)
            {
                var nombreArchivo = EmpleadoSeleccionado.FotografiaPath;
            }
            else
            {
                // emple.FotografiaBase64 = "img/Employee.png";
                EmpleadoSeleccionado.FotografiaPath = Url.Content("~/EmpleadoImg/Employee.png");
            }


            var viewModel = new EmpleadoViewModel
            {
                EmpleadoSeleccionado = EmpleadoSeleccionado,
                Empleados = Empleados,
                EmpleadosActivos = empleadosActivos

            };

            var nombreEmpleado = EmpleadoSeleccionado.NombreCompleto;
            ViewBag.NombreEmpleado = nombreEmpleado;
            ViewBag.IdEmpleado = EmpleadoSeleccionado.IdEmpleado;
            ViewBag.IdTipoNomina = EmpleadoSeleccionado.IdEmpleado;
            return View(viewModel);
        }

        [HttpGet]
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

        public ActionResult DownloadAll()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            List<EmpleadoActivo>? data = null;
            if (data == null)
            {
                data = _context.EmpleadoActivos.ToList();
            }
            DataTable table = converter.ToDataTable(data);
            string fileName = "EmpleadoActivo.xlsx";
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
                var nombreArchivo = emple.FotografiaName;
                //emple.FotografiaBase64 = Url.Content("~/img/Employee.png");
            }
            else
            {
                // emple.FotografiaBase64 = "img/Employee.png";
                emple.FotografiaName = Url.Content("~/EmpleadoImg/Employee.png");
            }



            var empleado = await _context.Empleados
                .Include(e => e.IdBancoNavigation)
                .Include(e => e.IdCargoNavigation)
                .Include(e => e.IdDepartamentoNavigation)
                .Include(e => e.IdEncargadoNavigation)
                .Include(e => e.IdTipoContratoNavigation)
                .Include(e => e.IdTipoNominaNavigation)
                .Include(e => e.IdClaseEmpleadoNavigation)
                .Include(e => e.IdUbicacionNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        [HttpGet]
        public async Task<IActionResult> LoadIngreso(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;

                var query = _context.EmpleadoIngresos.Where(e => e.IdEmpleado == id && e.Activo == true);
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.IdIngresoNavigation.NombreIngreso.Contains(filter));
                }

                var registros = await query.ToListAsync();
                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                return PartialView("~/Views/EmpleadoIngreso/Index.cshtml", registros);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadImpuesto(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;

                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                var IdCargoNavigation = await _context.Cargos.ToListAsync();
                var IdTipoContratoNavigation = await _context.TipoContratos.ToListAsync();

                var query = _context.EmpleadoImpuestos.Where(e => e.IdEmpleado == id && e.Activo == true);

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.IdEmpleadoNavigation.NombreCompleto.Contains(filter));
                }

                var registros = await query.ToListAsync();

                return PartialView("~/Views/EmpleadoImpuesto/Index.cshtml", registros);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadDeduccion(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;
                var query = _context.EmpleadoDeduccions.Where(e => e.IdEmpleado == id && e.Activo == true);
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.IdDeduccionNavigation.NombreDeduccion.Contains(filter));
                }

                var registros = await query.ToListAsync();
                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                return PartialView("~/Views/EmpleadoDeduccion/Index.cshtml", registros);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
       
    }
}
