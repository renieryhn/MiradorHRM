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

using PlanillaPM.Models;
using static PlanillaPM.Models.Ingreso;





namespace PlanillaPM.Controllers
{
    public class IngresoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public IngresoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       
        public async Task<IActionResult> Index(int pg, string? filter)
        {
            ViewBag.Filter = filter;
            List<Ingreso> registros;
            if (filter != null)
            {
                registros = await _context.Ingresos.Where(r => r.NombreIngreso.ToLower().Contains(filter.ToLower())).ToListAsync();
            }
            else
            {
                registros = await _context.Ingresos.OrderBy(r => r.Orden).ToListAsync(); // Ordenar por el campo Orden
            }

            const int pageSize = 10;
            if (pg < 1) pg = 1;
            int recsCount = registros.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }
        [HttpGet]
        public ActionResult Download()
        {
            // Obtener la lista de todos los ingresos
            var data = _context.Ingresos
                        .Select(i => new
                        {
                            i.IdIngreso,
                            NombreIngreso = i.NombreIngreso,
                            TipoIngreso = i.TipoIngreso.ToString(), 
                            TipoCalculo = i.TipoCalculo.ToString(), 
                            Monto = i.Monto.HasValue ? i.Monto.Value.ToString("F2") : "",
                            Formula = i.Formula,
                            Grabable = i.Grabable ? "Sí" : "No",
                            AsignacionAutomatica = i.AsignacionAutomatica ? "Sí" : "No",
                            Orden = i.Orden,
                            Activo = i.Activo ? "Sí" : "No",                          
                            FechaInicial = i.FechaInicial.HasValue ? i.FechaInicial.Value.ToString("dd/MM/yyyy") : "",
                            FechaFinal = i.FechaFinal.HasValue ? i.FechaFinal.Value.ToString("dd/MM/yyyy") : "",
                            Periodo = i.Periodo
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
            string fileName = "Ingresos.xlsx";

            // Crear el archivo de Excel y guardarlo en una secuencia de memoria
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(table, "Ingresos");
                worksheet.Columns().AdjustToContents(); // Ajustar el ancho de las columnas automáticamente

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // GET: Ingreso/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingreso = await _context.Ingresos
                .FirstOrDefaultAsync(m => m.IdIngreso == id);
            if (ingreso == null)
            {
                return NotFound();
            }

            return View(ingreso);
        }

        // GET: Ingreso/Create
        public IActionResult Create()
        {
            // Obtener el último valor del campo Orden
            int maxOrden = _context.Ingresos.Max(i => (int?)i.Orden) ?? 0;

            // Asignar el próximo valor de Orden al ViewBag
            ViewBag.NextOrden = maxOrden + 1;

            ViewBag.TipoCalculoEstado = Enum.GetValues(typeof(TipoCalculoEstado));

            // Obtener lista de TipoIngresoEstado
            ViewBag.TipoIngresoEstado = Enum.GetValues(typeof(TipoIngresoEstado))
                                              .Cast<TipoIngresoEstado>()
                                              .Select(e => new SelectListItem
                                              {
                                                  Value = ((int)e).ToString(),
                                                  Text = e.GetDisplayName()
                                              })
                                              .ToList();



            return View();
        }

        // POST: Ingreso/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdIngreso,NombreIngreso,TipoIngreso,TipoCalculo,Monto,Formula,Grabable,AsignacionAutomatica,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor,FechaInicial,FechaFinal,Periodo")] Ingreso ingreso, int? id)
        {
            if (ModelState.IsValid)
            {
                SetCamposAuditoria(ingreso, true);
                _context.Add(ingreso);
                await _context.SaveChangesAsync();
                TempData["success"] = "El registro ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["error"] = "Error: " + message;
            }
            return View(ingreso);
        }

        // GET: Ingreso/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingreso = await _context.Ingresos.FindAsync(id);
            if (ingreso == null)
            {
                return NotFound();
            }

            ViewBag.TipoCalculoEstado = Enum.GetValues(typeof(Ingreso.TipoCalculoEstado));
            ViewBag.TipoIngresoEstado = Enum.GetValues(typeof(TipoIngresoEstado))
                                  .Cast<TipoIngresoEstado>()
                                  .Select(e => new SelectListItem
                                  {
                                      Value = ((int)e).ToString(),
                                      Text = e.GetDisplayName()
                                  })
                                  .ToList();

          
            return View(ingreso);
        }

        // POST: Ingreso/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdIngreso,NombreIngreso,TipoIngreso,TipoCalculo,Monto,Formula,Grabable,AsignacionAutomatica,Orden,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor,FechaInicial,FechaFinal,Periodo")] Ingreso ingreso)
        {
            if (id != ingreso.IdIngreso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SetCamposAuditoria(ingreso, false);
                    _context.Update(ingreso);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngresoExists(ingreso.IdIngreso))
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
            ViewBag.TipoIngresoEstado = Enum.GetValues(typeof(TipoIngresoEstado))
                                  .Cast<TipoIngresoEstado>()
                                  .Select(e => new SelectListItem
                                  {
                                      Value = ((int)e).ToString(),
                                      Text = e.GetDisplayName()
                                  })
                                  .ToList();

            


            return View(ingreso);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(int[] order)
        {
            for (int i = 0; i < order.Length; i++)
            {
                var ingreso = await _context.Ingresos.FindAsync(order[i]);
                if (ingreso != null)
                {
                    ingreso.Orden = i + 1; // Actualiza el campo Orden según el nuevo orden
                    _context.Entry(ingreso).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }



        // GET: Ingreso/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingreso = await _context.Ingresos
                .FirstOrDefaultAsync(m => m.IdIngreso == id);
            if (ingreso == null)
            {
                return NotFound();
            }

            return View(ingreso);
        }

        // POST: Ingreso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var ingreso = await _context.Ingresos.FindAsync(id);
            try
            {
               
                if (ingreso != null)
                {
                    _context.Ingresos.Remove(ingreso);
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
                return View(ingreso);
            }

        }

        private bool IngresoExists(int id)
        {
            return _context.Ingresos.Any(e => e.IdIngreso == id);
        }
        
        private void SetCamposAuditoria(Ingreso record, bool bNewRecord)
        {
            var now = DateTime.Now;
            var CurrentUser =  _userManager.GetUserName(User);
           
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
