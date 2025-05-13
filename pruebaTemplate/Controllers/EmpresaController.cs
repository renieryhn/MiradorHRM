using System.Data;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static PlanillaPM.cGeneralFun;
using Microsoft.AspNetCore.Identity;
using PlanillaPM.ViewModel;
using System.Text;
using System.Text.Encodings.Web;
using PlanillaPM.Models;

namespace PlanillaPM.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;
        private IWebHostEnvironment Environment;

        public EmpresaController(PlanillaContext context, UserManager<Usuario> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            Environment = environment;
        }

        // GET: Empresa
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            ViewBag.Filter = filter;

            List<Empresa> registros;
            if (filter != null)
            {
                registros = await _context.Empresas.Where(r => r.NombreEmpresa.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Empresas.ToListAsync();
            }
            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            var planillaContext = _context.Empresas.Include(e => e.IdMonedaNavigation);

            var IdMonedaNavigation = await _context.Moneda.ToListAsync();
            return View(data);
        }
        [HttpGet]
        public ActionResult Download(string? filter)
        {
            var empresasQuery = _context.Empresas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                empresasQuery = empresasQuery
                    .Where(e => e.NombreEmpresa.ToLower().Contains(filter.ToLower()));
            }

            var data = empresasQuery
                .Select(e => new
                {
                    e.IdEmpresa,
                    Nombre = e.NombreEmpresa,
                    Moneda = e.IdMonedaNavigation.NombreMoneda,
                    Dirección = e.Direccion,
                    Teléfono = e.Telefono,
                    Email = e.Email,
                    RTN = e.Rtn,
                    Contacto = e.NombreContacto,
                    e.TelefonoContacto,
                    Activo = e.Activo ? "Sí" : "No",
                    e.Comentarios
                })
                .ToList();


            // Verificar si la lista está vacía
            if (!data.Any())
            {
                TempData["error"] = "No se encontraron Registros.";
                return RedirectToAction(nameof(Index));
            }

            // Convertir la lista en una tabla de datos
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable table = converter.ToDataTable(data);

            // Nombre del archivo de Excel
            string fileName = "Empresas.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Empresas");
                worksheet.Columns().AdjustToContents(); // Ajustar el ancho de las columnas

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // GET: Empresa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .Include(e => e.IdMonedaNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpresa == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // GET: Empresa/Create
        public IActionResult Create()
        {
            ViewData["IdMoneda"] = new SelectList(_context.Moneda, "IdMoneda", "NombreMoneda");
            return View();
        }

        // POST: Empresa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreEmpresa,IdMoneda,Direccion,Telefono,Email,Rtn,Comentarios,Activo,CreadoPor,ModificadoPor,NombreContacto,TelefonoContacto")] Empresa empresa, IFormFile Logo)
        {
            SetCamposAuditoria(empresa, true);
            if (ModelState.IsValid)
            {
              if (Logo != null && Logo.Length > 0)
                {
                    // Genera un nombre único para el archivo de imagen
                    var fileName = Guid.NewGuid() + System.IO.Path.GetExtension(Logo.FileName);
                    // Obtiene la ruta del directorio wwwroot/images
                    var imagePath = System.IO.Path.Combine(Environment.WebRootPath, "img", fileName);
                    // Copia el contenido del archivo a la ubicación en el servidor
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await Logo.CopyToAsync(stream);
                    }
                    empresa.LogoName = fileName;
                    empresa.LogoPath = "/img/" + fileName;
                }

                if (empresa.LogoPath == null)
                {
                    empresa.LogoPath = "/img/MiEmpresa.jpeg";
                }
                _context.Add(empresa);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            ViewData["IdMoneda"] = new SelectList(_context.Moneda, "IdMoneda", "NombreMoneda", empresa.IdMoneda);
            return View(empresa);
        }

        // GET: Empresa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null)
            {
                return NotFound();
            }
            if (empresa.LogoName == null)
            {
                empresa.LogoPath = Url.Content("~/img/MiEmpresa.jpeg");
            }
            ViewData["IdMoneda"] = new SelectList(_context.Moneda, "IdMoneda", "NombreMoneda", empresa.IdMoneda);
            return View(empresa);
        }

        // POST: Empresa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpresa,NombreEmpresa,IdMoneda,Direccion,Telefono,Email,Rtn,Comentarios,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor,NombreContacto,TelefonoContacto")] Empresa empresa, IFormFile Logo)
        {
            if (id != empresa.IdEmpresa)
            {
                return NotFound();
            }
            string previous_path = "";
            SetCamposAuditoria(empresa, false);
            if (ModelState.IsValid)
            {
                try
                {
                   
                    if (empresa.LogoName != null)
                    {
                        previous_path = System.IO.Path.Combine(Environment.WebRootPath, "img", empresa.LogoName);
                    }

                    if (Logo != null && Logo.Length > 0)
                    {
                        // Genera un nombre único para el archivo de imagen
                        var fileName = Guid.NewGuid() + System.IO.Path.GetExtension(Logo.FileName);
                        // Obtiene la ruta del directorio wwwroot/images
                        var imagePath = System.IO.Path.Combine(Environment.WebRootPath, "img", fileName);
                        // Copia el contenido del archivo a la ubicación en el servidor
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await Logo.CopyToAsync(stream);
                        }
                        empresa.LogoName = fileName;
                        empresa.LogoPath = "/img/" + fileName;
                        //elimina la imagen anterior
                        if (!string.IsNullOrEmpty(previous_path))
                        {
                            if (System.IO.File.Exists(previous_path))
                            {
                                System.IO.File.Delete(previous_path);
                            }
                        }
                    }

                    if (empresa.LogoPath == null)
                    {
                        empresa.LogoPath = "/img/MiEmpresa.jpeg";
                    }

                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresaExists(empresa.IdEmpresa))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }            
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["Error"] = "Error: " + message;
            }
            ViewData["IdMoneda"] = new SelectList(_context.Moneda, "IdMoneda", "NombreMoneda", empresa.IdMoneda);
            return View(empresa);
        }

        // GET: Empresa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .Include(e => e.IdMonedaNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpresa == id);
            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // POST: Empresa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var empresa = await _context.Empresas.FindAsync(id);
            try
            {
               
                if (empresa != null)
                {
                    _context.Empresas.Remove(empresa);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha sido eliminado exitosamente.";
                    return RedirectToAction(nameof(Index));
                } 
                else
                {
                    TempData["Error"] = "Hubo un error al intentar eliminar el Empleado Contacto. Por favor, verifica la información e intenta nuevamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_"))
                {
                    TempData["error"] = "Error: No puede elimiar el registro actual ya que se encuentra relacionado a otro Registro.";
                }
                else
                {
                    var message = ex.InnerException;
                    TempData["error"] = "Error: " + message;
                }
                return View(empresa);
            }

        }

        private bool EmpresaExists(int id)
        {
            return _context.Empresas.Any(e => e.IdEmpresa == id);
        }
        
        private void SetCamposAuditoria(Empresa record, bool bNewRecord)
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
    }
}
