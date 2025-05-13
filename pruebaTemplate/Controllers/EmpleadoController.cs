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



namespace PlanillaPM.Controllers
{

    public class EmpleadoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Usuario> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IWebHostEnvironment Environment;


        public EmpleadoController(PlanillaContext context, Microsoft.AspNetCore.Identity.UserManager<Usuario> userManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Environment = environment;


        }

        [HttpPost]
        public async Task<IActionResult> PasarIDConstancia1(int id)
        {

            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            return RedirectToAction("Constancia1", "Constancia", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> PasarIDConstancia2(int id)
        {

            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            return RedirectToAction("Constancia2", "Constancia", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> PasarIDConstanciaTrabajo(int id)
        {

            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            return RedirectToAction("ConstanciaTrabajo", "Constancia", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> pasarIDDFContrato(int id)
        {

            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            return RedirectToAction("FContrato", "Constancia", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> pasarIDDFContratoCorto(int id)
        {

            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            return RedirectToAction("FContratoCorto", "Constancia", new { id = id });
        }

        [AllowAnonymous]
        public IActionResult ExportToPDFConstancia1(int id)
        {

            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();


            int widthInPoints = (int)(8.5 * 72);
            int heightInPoints = (int)(11 * 72);


            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(widthInPoints, heightInPoints);


            htmlConverter.ConverterSettings = blinkConverterSettings;


            string url = $"https://localhost:7021/Constancia/Constancia1/{id}";


            using (PdfDocument document = htmlConverter.Convert(url))
            {

                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;


                return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "Constancia1.pdf");
            }
        }
        public IActionResult ExportToPDFConstancia2(int id)
        {

            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();


            int widthInPoints = (int)(8.5 * 72);
            int heightInPoints = (int)(11 * 72);


            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(widthInPoints, heightInPoints);


            htmlConverter.ConverterSettings = blinkConverterSettings;


            string url = $"https://localhost:7021/Constancia/Constancia2/{id}";


            using (PdfDocument document = htmlConverter.Convert(url))
            {

                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;


                return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "Constancia2.pdf");
            }
        }
        public IActionResult ExportToPDFConstanciaTrabajo(int id)
        {

            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();


            int widthInPoints = (int)(8.5 * 72);
            int heightInPoints = (int)(11 * 72);


            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(widthInPoints, heightInPoints);


            htmlConverter.ConverterSettings = blinkConverterSettings;


            string url = $"https://localhost:7021/Constancia/ConstanciaTrabajo/{id}";


            using (PdfDocument document = htmlConverter.Convert(url))
            {

                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;


                return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "Constancia de Trabajo.pdf");
            }
        }
        public IActionResult ExportToPDFContrato(int id)
        {

            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();


            int widthInPoints = (int)(8.5 * 72);
            int heightInPoints = (int)(11 * 72);


            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(widthInPoints, heightInPoints);


            htmlConverter.ConverterSettings = blinkConverterSettings;


            string url = $"https://localhost:7021/Constancia/FContrato/{id}";


            using (PdfDocument document = htmlConverter.Convert(url))
            {

                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;


                return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "Contrato.pdf");
            }
        }
        public IActionResult ExportToPDFContratoCorto(int id)
        {

            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();


            int widthInPoints = (int)(8.5 * 72);
            int heightInPoints = (int)(11 * 72);


            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(widthInPoints, heightInPoints);


            htmlConverter.ConverterSettings = blinkConverterSettings;


            string url = $"https://localhost:7021/Constancia/FContratoCorto/{id}";


            using (PdfDocument document = htmlConverter.Convert(url))
            {

                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;


                return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "Contrato Corto.pdf");
            }
        }


        // GET: Empleado
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            try
            {
                ViewBag.Filter = filter;

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
                    if (empleado.FotografiaName != null)
                    {
                        var nombreArchivo = empleado.FotografiaPath;

                    }
                    else
                    {
                        empleado.FotografiaPath = "EmpleadoImg/Employee.png";
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
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un problema al cargar la página." + ex.Message;
                return View();
            }
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


        public ActionResult Download(string? filter)
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();

            // Aplicar el filtro como en Index
            var empleadosQuery = _context.Empleados.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                empleadosQuery = empleadosQuery.Where(e => e.NombreEmpleado.ToLower().Contains(filter.ToLower()));
            }
            // Obtener la lista de todos los empleados
            var data = empleadosQuery
         .Select(e => new
         {
             e.IdEmpleado,
             CodigoInterno = e.CodigoInterno ?? "N/A",
             NombreCompleto = $"{e.NombreEmpleado} {e.ApellidoEmpleado}",
             NumeroIdentidad = e.NumeroIdentidad ?? "N/A",
             NumeroLicencia = e.NumeroLicencia ?? "N/A",
             FechaVencimientoLicencia = e.FechaVencimientoLicencia.HasValue ? e.FechaVencimientoLicencia.Value.ToString("dd/MM/yyyy") : "N/A",
             Nacionalidad = e.Nacionalidad,
             FechaNacimiento = e.FechaNacimiento.ToString("dd/MM/yyyy"),
             Edad = e.Edad,
             Genero = e.Genero ?? "N/A",
             Direccion = e.Direccion ?? "N/A",
             Telefono = e.Telefono,
             CiudadResidencia = e.CiudadResidencia,
             Email = e.Email ?? "N/A",
             Activo = e.Activo ? "Sí" : "No",
             Cargo = e.IdCargoNavigation.NombreCargo,
             Departamento = e.IdDepartamentoNavigation.NombreDepartamento,
             TipoContrato = e.IdTipoContratoNavigation.NombreTipoContrato,
             TipoNomina = e.IdTipoNominaNavigation != null ? e.IdTipoNominaNavigation.NombreTipoNomina : "N/A",
             FechaInicio = e.FechaInicio.HasValue ? e.FechaInicio.Value.ToString("dd/MM/yyyy") : "N/A",
             SalarioBase = e.SalarioBase,
             Antiguedad = e.Antiguedad,
             NumeroSeguroSocial = e.NumeroSeguroSocial ?? "N/A",
             e.Comentarios
         })
         .ToList();


            // Verificar si la lista está vacía
            if (!data.Any())
            {
                TempData["error"] = "No se encontraron Registros.";
                return RedirectToAction(nameof(Index));
            }

            // Convertir la lista a DataTable
            DataTable table = converter.ToDataTable(data);

            // Definir el nombre del archivo
            string fileName = "Empleados.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Agregar DataTable al workbook
                wb.Worksheets.Add(table, "Empleados");

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    // Retornar el archivo Excel
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

        // GET: Empleado/Create
        public IActionResult Create()
        {

            ViewBag.EstadoCivilEmpleado = Enum.GetValues(typeof(EstadoCivilEmpleado));
            ViewBag.TipoCuentaBancaria = Enum.GetValues(typeof(TipoCuenta));
            ViewData["IdBanco"] = new SelectList(_context.Bancos.Where(r => r.Activo), "IdBanco", "NombreBanco");
            ViewData["IdCargo"] = new SelectList(_context.Cargos.Where(r => r.Activo), "IdCargo", "NombreCargo");
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos.Where(r => r.Activo), "IdDepartamento", "NombreDepartamento");
            ViewData["IdEncargado"] = new SelectList(_context.Empleados.Where(r => r.Activo), "IdEmpleado", "NombreCompleto");
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos.Where(r => r.Activo), "IdTipoContrato", "NombreTipoContrato");
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas.Where(r => r.Activo), "IdTipoNomina", "NombreTipoNomina");
            ViewData["IdUbicacion"] = new SelectList(_context.Ubicaciones.Where(r => r.Activo), "IdUbicacion", "NombreUbicacion");
            ViewData["IdClaseEmpleado"] = new SelectList(_context.ClaseEmpleados.Where(r => r.Activo), "IdClaseEmpleado", "NombreClaseEmpleado");
            return View();
        }

        // POST: Empleado/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmpleado,CodigoInterno,NombreEmpleado,ApellidoEmpleado,NumeroIdentidad,NumeroLicencia,FechaVencimientoLicencia,Nacionalidad,FechaNacimiento,Genero,Direccion,Telefono,CiudadResidencia,Email,Activo,IdCargo,IdDepartamento,IdTipoContrato,IdTipoNomina,IdEncargado,IdClaseEmpleado,IdUbicacion,EstadoCivil,FechaInicio,IdBanco,TipoCuentaBancaria,CuentaBancaria,NumeroRegistroTributario,SalarioBase,NumeroSeguroSocial,Comentarios,Observaciones,FechaInactivacion,MotivoInactivacion")] Empleado empleado, IFormFile Fotografia)
        {

            try
            {

                SetCamposAuditoria(empleado, true);
                // Verificar si el IdBanco seleccionado es "Seleccionar" y establecerlo como null
                if (empleado.IdBanco == 0)
                {
                    empleado.IdBanco = null;
                }

                if (ModelState.IsValid)
                {

                    if (Fotografia != null && Fotografia.Length > 0)
                    {
                        // Genera un nombre único para el archivo de imagen
                        var fileName = Guid.NewGuid() + System.IO.Path.GetExtension(Fotografia.FileName);
                        // Obtiene la ruta del directorio wwwroot/images
                        var imagePath = System.IO.Path.Combine(Environment.WebRootPath, "EmpleadoImg", fileName);
                        // Copia el contenido del archivo a la ubicación en el servidor
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await Fotografia.CopyToAsync(stream);
                        }
                        empleado.FotografiaName = fileName;
                        empleado.FotografiaPath = "/EmpleadoImg/" + fileName;
                    }
                    else
                    {
                        empleado.FotografiaName = "Employee.png"; // Establece un nombre de imagen predeterminado si no se proporciona ninguna imagen
                        empleado.FotografiaPath = "/EmpleadoImg/" + empleado.FotografiaName; // Establece la ruta de la imagen predeterminada
                    }
                    _context.Add(empleado);
                    await _context.SaveChangesAsync();

                    TempData["mensaje"] = "Empleado creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["Error"] = "Error: " + message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un problema al intentar crear el registro. Por favor, intente nuevamente. " + ex.Message;
                // TempData["error"] = "Error: " + ex.Message;
            }


            ViewData["IdBanco"] = new SelectList(_context.Bancos.Where(r => r.Activo), "IdBanco", "NombreBanco");
            ViewData["IdCargo"] = new SelectList(_context.Cargos.Where(r => r.Activo), "IdCargo", "NombreCargo");
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos.Where(r => r.Activo), "IdDepartamento", "NombreDepartamento");
            ViewData["IdEncargado"] = new SelectList(_context.Empleados.Where(r => r.Activo), "IdEmpleado", "NombreCompleto");
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos.Where(r => r.Activo), "IdTipoContrato", "NombreTipoContrato");
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas.Where(r => r.Activo), "IdTipoNomina", "NombreTipoNomina");
            ViewData["IdUbicacion"] = new SelectList(_context.Ubicaciones.Where(r => r.Activo), "IdUbicacion", "NombreUbicacion");
            ViewData["IdClaseEmpleado"] = new SelectList(_context.ClaseEmpleados.Where(r => r.Activo), "IdClaseEmpleado", "NombreClaseEmpleado");
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
                var nombreArchivo = empleado.FotografiaName;

            }
            else
            {
                // emple.FotografiaBase64 = "img/Employee.png";
                empleado.FotografiaName = Url.Content("~/EmpleadoImg/Employee.png");
            }




            ViewBag.EstadoCivilEmpleado = Enum.GetValues(typeof(Empleado.EstadoCivilEmpleado));
            ViewBag.TipoCuentaBancaria = Enum.GetValues(typeof(TipoCuenta));
            ViewData["IdBanco"] = new SelectList(_context.Bancos.Where(r => r.Activo), "IdBanco", "NombreBanco", empleado.IdBanco);
            ViewData["IdCargo"] = new SelectList(_context.Cargos.Where(r => r.Activo), "IdCargo", "NombreCargo", empleado.IdCargo);
            ViewData["IdDepartamento"] = new SelectList(_context.Departamentos.Where(r => r.Activo), "IdDepartamento", "NombreDepartamento", empleado.IdDepartamento);
            ViewData["IdEncargado"] = new SelectList(_context.Empleados.Where(r => r.Activo), "IdEmpleado", "NombreCompleto", empleado.IdEncargado);
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos.Where(r => r.Activo), "IdTipoContrato", "NombreTipoContrato", empleado.IdTipoContrato);
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas.Where(r => r.Activo), "IdTipoNomina", "NombreTipoNomina", empleado.IdTipoNomina);
            ViewData["IdUbicacion"] = new SelectList(_context.Ubicaciones.Where(r => r.Activo), "IdUbicacion", "NombreUbicacion", empleado.IdUbicacion);
            ViewData["IdClaseEmpleado"] = new SelectList(_context.ClaseEmpleados.Where(r => r.Activo), "IdClaseEmpleado", "NombreClaseEmpleado", empleado.IdClaseEmpleado);
            return View(empleado);
        }

        // POST: Empleado/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleado,CodigoInterno,NombreEmpleado,ApellidoEmpleado,NumeroIdentidad,NumeroLicencia,FechaVencimientoLicencia,Nacionalidad,FechaNacimiento,Genero,Direccion,Telefono,CiudadResidencia,Email,Activo,IdCargo,IdDepartamento,IdTipoContrato,IdTipoNomina,IdEncargado,IdClaseEmpleado,IdUbicacion,EstadoCivil,FechaInicio,IdBanco,TipoCuentaBancaria,CuentaBancaria,NumeroRegistroTributario,SalarioBase,NumeroSeguroSocial,Comentarios,Observaciones,FechaInactivacion,MotivoInactivacion,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor")] Empleado empleado, IFormFile Fotografia, bool IsImageRemoved)
        {


            try
            {

                if (id != empleado.IdEmpleado)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    // Cargar el valor original de FotografiaPath
                    var empleadoOriginal = await _context.Empleados.AsNoTracking().FirstOrDefaultAsync(e => e.IdEmpleado == id);
                    if (empleadoOriginal == null)
                    {
                        return NotFound();
                    }

                    SetCamposAuditoria(empleado, false);
                    if (IsImageRemoved)
                    {
                        empleado.FotografiaName = "Employee.png";
                        empleado.FotografiaPath = "/EmpleadoImg/Employee.png";
                    }
                    else if (Fotografia != null && Fotografia.Length > 0)
                    {
                        // Genera un nombre único para el archivo de imagen
                        var fileName = Guid.NewGuid() + System.IO.Path.GetExtension(Fotografia.FileName);
                        // Obtiene la ruta del directorio wwwroot/images
                        var imagePath = System.IO.Path.Combine(Environment.WebRootPath, "EmpleadoImg", fileName);
                        // Copia el contenido del archivo a la ubicación en el servidor
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await Fotografia.CopyToAsync(stream);
                        }
                        empleado.FotografiaName = fileName;
                        empleado.FotografiaPath = "/EmpleadoImg/" + fileName;
                    }
                    else
                    {
                        // Mantener el valor original de FotografiaPath si no se carga una nueva imagen
                        empleado.FotografiaName = empleadoOriginal.FotografiaName;
                        empleado.FotografiaPath = empleadoOriginal.FotografiaPath;
                    }

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
            ViewData["IdEncargado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto", empleado.IdEncargado);
            ViewData["IdTipoContrato"] = new SelectList(_context.TipoContratos, "IdTipoContrato", "NombreTipoContrato", empleado.IdTipoContrato);
            ViewData["IdTipoNomina"] = new SelectList(_context.TipoNominas, "IdTipoNomina", "NombreTipoNomina", empleado.IdTipoNomina);
            ViewData["IdUbicacion"] = new SelectList(_context.Ubicaciones.Where(r => r.Activo), "IdUbicacion", "NombreUbicacion", empleado.IdUbicacion);
            ViewData["IdClaseEmpleado"] = new SelectList(_context.ClaseEmpleados.Where(r => r.Activo), "IdClaseEmpleado", "NombreClaseEmpleado", empleado.IdClaseEmpleado);
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
                var nombreArchivo = emple.FotografiaName;
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


        public async Task<IActionResult> FichaEmpleado(int? id, bool? estado)
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
           //.Include(e => e.EmpleadoContactos)
           //.Include(e => e.EmpleadoContratos)
           //.Include(e => e.EmpleadoEducacions)
           //.Include(e => e.EmpleadoExperiencia)
           //.Include(e => e.EmpleadoHabilidads)
           //.Include(e => e.EmpleadoAusencia)
           //.Include(e => e.EmpleadoActivos)
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
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> LoadContactos(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;

                var query = _context.EmpleadoContactos.Where(e => e.IdEmpleado == id && e.Activo == true);
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.NombreContacto.Contains(filter));
                }

                var registros = await query.ToListAsync();
                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                return PartialView("~/Views/EmpleadoContacto/_EmpleadoContactoIndex.cshtml", registros);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadContratos(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;

                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                var IdCargoNavigation = await _context.Cargos.ToListAsync();
                var IdTipoContratoNavigation = await _context.TipoContratos.ToListAsync();

                var query = _context.EmpleadoContratos.Where(e => e.IdEmpleado == id && e.Activo == true);

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.IdCargoNavigation.EmpleadoContratos.Any(ec => ec.IdTipoContratoNavigation.NombreTipoContrato.Contains(filter)));
                }

                var registros = await query.ToListAsync();

                return PartialView("~/Views/EmpleadoContrato/_EmpleadoContratoIndex.cshtml", registros);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadEducacion(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;
                var query = _context.EmpleadoEducacions.Where(e => e.IdEmpleado == id && e.Activo == true);
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.Institucion.Contains(filter));
                }

                var registros = await query.ToListAsync();
                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                return PartialView("~/Views/EmpleadoEducacion/_EmpleadoEducacionIndex.cshtml", registros);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoadExperiencia(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;
                var query = _context.EmpleadoExperiencia.Where(e => e.IdEmpleado == id && e.Activo == true);
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.Empresa.Contains(filter));
                }

                var registros = await query.ToListAsync();
                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                return PartialView("~/Views/EmpleadoExperiencium/_EmpleadoExperienciumIndex.cshtml", registros);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoadHabilidad(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;

                var query = _context.EmpleadoHabilidads.Where(e => e.IdEmpleado == id && e.Activo == true);
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.Habilidad.Contains(filter));
                }

                var registros = await query.ToListAsync();
                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                return PartialView("~/Views/EmpleadoHabilidad/_EmpleadoHabilidadIndex.cshtml", registros);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoadAusencias(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;


                var query = _context.EmpleadoAusencia.Where(e => e.IdEmpleado == id && e.Activo == true);

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.IdTipoAusenciaNavigation.EmpleadoAusencia.Any(ec => ec.IdTipoAusenciaNavigation.NombreTipoAusencia.Contains(filter)));
                }

                var registros = await query.ToListAsync();
                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                var IdTipoAusenciaNavigation = await _context.TipoAusencia.ToListAsync();
                return PartialView("~/Views/EmpleadoAusencium/_EmpleadoAusenciumIndex.cshtml", registros);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoadEmpleado(int id, string filter)
        {
            try
            {
                ViewBag.IdEmpleado = id;
                var query = _context.EmpleadoActivos.Where(e => e.IdEmpleado == id && e.Activo == true);
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(e => e.IdProductoNavigation.EmpleadoActivos.Any(ec => ec.IdProductoNavigation.NombreProducto.Contains(filter)));
                }

                var registros = await query.ToListAsync();
                var IdEmpleadoNavigation = await _context.Empleados.ToListAsync();
                var IdProductoNavigation = await _context.Productos.ToListAsync();
                return PartialView("~/Views/EmpleadoActivo/_EmpleadoActivoIndex.cshtml", registros);
            }
            catch (Exception)
            {
                throw;
            }
        }




        private void SetCamposAuditoria(Empleado record, bool bNewRecord)
        {

            try
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
                if (record.FechaCreacion.ToString() == "1/1/0001 00:00:00")
                {
                    record.FechaCreacion = now;
                }
                if (record.CreadoPor == null)
                {
                    record.CreadoPor = CurrentUser;
                }
            }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
