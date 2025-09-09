using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiradorHRM.Models;
using Newtonsoft.Json;
using NuGet.DependencyResolver;
using PlanillaPM.Models;
using PlanillaPM.ViewModel;
using static PlanillaPM.cGeneralFun;

namespace PlanillaPM.Controllers
{
    public class EmpleadoHorasTrabajoController : Controller
    {
        private readonly PlanillaContext _context;
        private readonly UserManager<Usuario> _userManager;

        public EmpleadoHorasTrabajoController(PlanillaContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmpleadoHorasTrabajo
        //public async Task<IActionResult> Index(int pg, string? filter)
        //{
        //    List<EmpleadoHorasTrabajo> registros;
        //    if (filter != null)
        //    {
        //        registros = await _context.EmpleadoHorasTrabajos.Where(r => r.IdEmpleadoNavigation.NombreCompleto.ToLower().Contains(filter.ToLower())).ToListAsync();
        //    }
        //    else
        //    {
        //        registros = await _context.EmpleadoHorasTrabajos.ToListAsync();
        //    }
        //    const int pageSize = 10;
        //    if (pg < 1) pg = 1;
        //    int recsCount = registros.Count();
        //    var pager = new Pager(recsCount, pg, pageSize);
        //    int recSkip = (pg - 1) * pageSize;
        //    var data = registros.Skip(recSkip).Take(pager.PageSize).ToList();
        //    this.ViewBag.Pager = pager;
        //    return View(data);
        //}

        public async Task<IActionResult> Index(int pg, string? filter)
        {
            ViewBag.Filter = filter;

            IQueryable<EmpleadoHorasTrabajo> query = _context.EmpleadoHorasTrabajos
                .Include(e => e.IdEmpleadoNavigation)
                  .Include(e => e.IdEmpleadoHorarioNavigation)
                        .ThenInclude(h => h.IdHorarioBaseNavigation);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(r => EF.Functions.Like(r.IdEmpleadoNavigation.NombreEmpleado, $"%{filter}%"));
            }

            const int pageSize = 10;
            if (pg < 1) pg = 1;

            int recsCount = await query.CountAsync();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = await query
                .Skip(recSkip)
                .Take(pager.PageSize)
                .ToListAsync();

            ViewBag.Pager = pager;
            return View(data);
        }


        public ActionResult Download()
         {
             ListtoDataTableConverter converter = new ListtoDataTableConverter();
             List<EmpleadoHorasTrabajo>? data = null;
             if (data == null)
             {
                data = _context.EmpleadoHorasTrabajos.ToList();
             }
             DataTable table = converter.ToDataTable(data);
             string fileName = "EmpleadoHorasTrabajos.xlsx";
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
        // GET: EmpleadoHorasTrabajo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var registro = await _context.EmpleadoHorasTrabajos
                .Include(e => e.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(e => e.IdEmpleadoHorasTrabajo == id);

            if (registro == null) return NotFound();

            int mes = registro.Fecha.Month;
            int anio = registro.Fecha.Year;

            var registrosDelMes = await _context.EmpleadoHorasTrabajos
                .Include(e => e.IdEmpleadoNavigation)
                .Where(e => e.IdEmpleado == registro.IdEmpleado &&
                            e.Fecha.Month == mes &&
                            e.Fecha.Year == anio)
                .OrderBy(e => e.Fecha)
                .ToListAsync();

            var vm = new EmpleadoHorasTrabajoDetalleViewModel
            {
                RegistroActual = registro,
                RegistrosDelMes = registrosDelMes
            };

            return View(vm);
        }

        // GET: EmpleadoHorasTrabajo/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "NombreCompleto");
            return View();
        }

        public IActionResult ImportarDesdeArchivo(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Archivo no válido.");

            if (!Path.GetExtension(archivo.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos .xlsx");

            var resultado = new List<RegistroEmpleadoViewModel>();
            string dispositivoNombre = "---";


            using (var stream = new MemoryStream())
            {
                archivo.CopyTo(stream);
                stream.Position = 0;

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.First();
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    var registros = new List<(int CodigoReloj, string Nombre, DateTime Tiempo, string Estado, string Dispositivo)>();

                    foreach (var row in rows)
                    {
                        var codigoReloj = row.Cell(1).GetValue<int>();
                        var nombre = row.Cell(2).GetValue<string>()?.Trim();
                        var tiempoStr = row.Cell(3).GetValue<string>()?.Trim();
                        var estado = row.Cell(4).GetValue<string>()?.Trim();
                        var dispositivo = row.Cell(5).GetValue<string>()?.Trim() ?? "---";

                        if (!DateTime.TryParse(tiempoStr, out DateTime tiempo))
                            continue;

                        registros.Add((codigoReloj, nombre ?? "", tiempo, estado ?? "", dispositivo));
                    }

                    dispositivoNombre = registros.FirstOrDefault().Dispositivo ?? "---";

                    var registrosAgrupados = registros
                        .GroupBy(r => new { r.Tiempo.Date, r.CodigoReloj, r.Nombre })
                        .ToList();

               
                    foreach (var grupo in registrosAgrupados)
                    {
                       

                        // Obtener empleado
                        var empleado = _context.Empleados.FirstOrDefault(e => e.CodigoReloj == grupo.Key.CodigoReloj && e.Activo);
                        if (empleado == null)
                        {
                            continue;
                        }
                        

                        // Buscar el horario asignado al empleado en la tabla EmpleadoHorario
                        var empleadoHorario = _context.EmpleadoHorarios
                            .FirstOrDefault(h => h.IdEmpleado == empleado.IdEmpleado);

                        // Obtener el ID del horario base
                        var idHorarioBase = empleadoHorario?.IdHorarioBase;

                        string tipoJornada = "Desconocido";
                        if (idHorarioBase != null)
                        {
                            var horario = _context.Horarios.FirstOrDefault(h => h.IdHorario == idHorarioBase);
                            if (horario != null)
                                tipoJornada = horario.NombreHorario ?? "Desconocido";

                        }

                        var entradas = grupo.Where(g => g.Estado == "Entrada").Select(g => g.Tiempo).ToList();
                        var salidas = grupo.Where(g => g.Estado == "Salida").Select(g => g.Tiempo).ToList();
                        var recesos = grupo.Where(g => string.IsNullOrWhiteSpace(g.Estado)).Select(g => g.Tiempo).OrderBy(t => t).ToList();

                        var entrada = entradas.FirstOrDefault();
                        var salida = salidas.LastOrDefault();
                        var recesoDesde = recesos.FirstOrDefault();
                        var recesoHasta = recesos.LastOrDefault();

                        TimeSpan horasDiarias = TimeSpan.Zero;
                        if (entrada != default && salida != default)
                        {
                            horasDiarias = salida - entrada;
                            if (recesoDesde != default && recesoHasta != default && recesoHasta > recesoDesde)
                                horasDiarias -= (recesoHasta - recesoDesde);
                        }

                        var fecha = grupo.Key.Date;
                        var diaTexto = fecha.ToString("d MMM. dddd", new CultureInfo("es-ES"));
                        diaTexto = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(diaTexto);


                        // Determinar día de la semana
                        var diaSemana = fecha.DayOfWeek.ToString().Substring(0, 3).ToLower(); // lun, mar, mie...

                        TimeSpan? entradaEsperada = null;
                        TimeSpan? salidaEsperada = null;
                        TimeSpan? recesoEsperadoDesde = null;
                        TimeSpan? recesoEsperadoHasta = null;

                        switch (fecha.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                entradaEsperada = empleadoHorario?.LunDesde;
                                salidaEsperada = empleadoHorario?.LunHasta;
                                recesoEsperadoDesde = empleadoHorario?.LunRecesoDesde;
                                recesoEsperadoHasta = empleadoHorario?.LunRecesoHasta;
                                break;
                            case DayOfWeek.Tuesday:
                                entradaEsperada = empleadoHorario?.MarDesde;
                                salidaEsperada = empleadoHorario?.MarHasta;
                                recesoEsperadoDesde = empleadoHorario?.MarRecesoDesde;
                                recesoEsperadoHasta = empleadoHorario?.MarRecesoHasta;
                                break;
                            case DayOfWeek.Wednesday:
                                entradaEsperada = empleadoHorario?.MieDesde;
                                salidaEsperada = empleadoHorario?.MieHasta;
                                recesoEsperadoDesde = empleadoHorario?.MieRecesoDesde;
                                recesoEsperadoHasta = empleadoHorario?.MieRecesoHasta;
                                break;
                            case DayOfWeek.Thursday:
                                entradaEsperada = empleadoHorario?.JueDesde;
                                salidaEsperada = empleadoHorario?.JueHasta;
                                recesoEsperadoDesde = empleadoHorario?.JueRecesoDesde;
                                recesoEsperadoHasta = empleadoHorario?.JueRecesoHasta;
                                break;
                            case DayOfWeek.Friday:
                                entradaEsperada = empleadoHorario?.VieDesde;
                                salidaEsperada = empleadoHorario?.VieHasta;
                                recesoEsperadoDesde = empleadoHorario?.VieRecesoDesde;
                                recesoEsperadoHasta = empleadoHorario?.VieRecesoHasta;
                                break;
                            case DayOfWeek.Saturday:
                                entradaEsperada = empleadoHorario?.SabDesde;
                                salidaEsperada = empleadoHorario?.SabHasta;
                                recesoEsperadoDesde = empleadoHorario?.SabRecesoDesde;
                                recesoEsperadoHasta = empleadoHorario?.SabRecesoHasta;
                                break;
                            case DayOfWeek.Sunday:
                                entradaEsperada = empleadoHorario?.DomDesde;
                                salidaEsperada = empleadoHorario?.DomHasta;
                                recesoEsperadoDesde = empleadoHorario?.DomRecesoDesde;
                                recesoEsperadoHasta = empleadoHorario?.DomRecesoHasta;
                                break;
                        }

                        // Asignar por defecto si no se detectó en el archivo
                        var partesMotivo = new List<string>();

                        if (entrada == default && entradaEsperada.HasValue)
                        {
                            entrada = fecha.Date + entradaEsperada.Value;
                            partesMotivo.Add("Entrada");
                        }
                        if (salida == default && salidaEsperada.HasValue)
                        {
                            salida = fecha.Date + salidaEsperada.Value;
                            partesMotivo.Add("Salida");
                        }
                        if (recesoDesde == default && recesoEsperadoDesde.HasValue)
                        {
                            recesoDesde = fecha.Date + recesoEsperadoDesde.Value;
                            partesMotivo.Add("R-Entrada");
                        }
                        if (recesoHasta == default && recesoEsperadoHasta.HasValue)
                        {
                            recesoHasta = fecha.Date + recesoEsperadoHasta.Value;
                            partesMotivo.Add("R-Salida");
                        }


                        string motivoCaptura;

                        if (partesMotivo.Count == 0)
                        {
                            motivoCaptura = "Reloj";
                        }
                        else if (partesMotivo.Count == 1)
                        {
                            motivoCaptura = "Auto " + partesMotivo[0];
                        }
                        else if (partesMotivo.Count == 2)
                        {
                            motivoCaptura = "Auto " + string.Join(" y ", partesMotivo);
                        }
                        else
                        {
                            string todasMenosUltima = string.Join(", ", partesMotivo.Take(partesMotivo.Count - 1));
                            string ultima = partesMotivo.Last();
                            motivoCaptura = $"Auto {todasMenosUltima} y {ultima}";
                        }


                        // Recalcular horas
                        horasDiarias = TimeSpan.Zero;
                        if (entrada != default && salida != default)
                        {
                            horasDiarias = salida - entrada;
                            if (recesoDesde != default && recesoHasta != default && recesoHasta > recesoDesde)
                                horasDiarias -= (recesoHasta - recesoDesde);
                        }

                        // Si no hay jornada asignada y no se autocompletó nada, mostrar "Desconocido"
                        if ((string.IsNullOrWhiteSpace(tipoJornada) || tipoJornada == "Desconocido") && partesMotivo.Count == 0)
                        {
                            motivoCaptura = "Desconocido";
                        }


                        resultado.Add(new RegistroEmpleadoViewModel
                        {
                            IdEmpleado = empleado.IdEmpleado,
                            IdEmpleadoHorario = empleadoHorario?.IdEmpleadoHorario ?? 0,
                            Nombre = empleado.NombreCompleto,
                            Dia = diaTexto,
                            Fecha = fecha,
                            Entrada = entrada != default ? entrada.ToString("HH:mm") : "00:00",
                            Salida = salida != default ? salida.ToString("HH:mm") : "00:00",
                            RecesoDesde = recesoDesde != default ? recesoDesde.ToString("HH:mm") : "00:00",
                            RecesoHasta = recesoHasta != default ? recesoHasta.ToString("HH:mm") : "00:00",
                            Horas = horasDiarias > TimeSpan.Zero ? $"{(int)horasDiarias.TotalHours}:{horasDiarias.Minutes:D2}" : "0:00",
                            TipoJornada = tipoJornada,
                            MotivoCaptura = motivoCaptura

                        });
                    }
                }
            }


            // Ordenar globalmente por fecha
            return Json(new
            {
                Registros = resultado.OrderBy(r => r.Fecha).ToList(),
                Dispositivo = dispositivoNombre
            });
        }



        // POST: EmpleadoHorasTrabajo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(100_000_000)]
        //public async Task<IActionResult> Create([Bind("IdEmpleadoHorasTrabajo,IdEmpleado,Fecha,Entrada,Salida,Receso_Desde,Receso_Hasta,IdEmpleadoHorario,TotalNormales,TotalDiurna,TotalNocturna,TotalMixta,TotalNoTrabajado,Estado,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor,Dispositivo,DiaSemana,TotalHorasReloj")] EmpleadoHorasTrabajo empleadoHorasTrabajo, string JsonRegistros)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(JsonRegistros))
        //            return BadRequest("No se recibió información de registros (JsonRegistros está vacío o nulo).");

        //        List<EmpleadoHorasTrabajoInputModel> registrosInput;
        //        try
        //        {
        //            registrosInput = JsonConvert.DeserializeObject<List<EmpleadoHorasTrabajoInputModel>>(JsonRegistros);
        //        }
        //        catch (Exception jsonEx)
        //        {
        //            return BadRequest("Error al deserializar JsonRegistros: " + jsonEx.Message);
        //        }

        //        empleadoHorasTrabajo.Fecha = DateTime.Now;
        //        empleadoHorasTrabajo.Estado = "Pendiente";

        //        if (registrosInput == null || !registrosInput.Any())
        //            return BadRequest("El JsonRegistros no contiene registros válidos.");

        //        if (empleadoHorasTrabajo.IdEmpleado == 0)
        //            empleadoHorasTrabajo.IdEmpleado = registrosInput.First().IdEmpleado;

        //        if (string.IsNullOrWhiteSpace(empleadoHorasTrabajo.Estado))
        //            empleadoHorasTrabajo.Estado = "Pendiente";




        //        var usuarioActual = _userManager.GetUserName(User);
        //        SetCamposAuditoria(empleadoHorasTrabajo, true, usuarioActual);

        //        if (!ModelState.IsValid)
        //        {
        //            var errores = ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage);

        //            return BadRequest("Error de validación: " + string.Join(" | ", errores));
        //        }

        //        //_context.Add(empleadoHorasTrabajo);

        //        foreach (var input in registrosInput)
        //        {

        //            var detalle = new EmpleadoHorasTrabajo
        //            {
        //                IdEmpleado = input.IdEmpleado,
        //                Fecha = empleadoHorasTrabajo.Fecha, // La fecha general del encabezado, no del input
        //                DiaSemana = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
        //    input.Fecha.ToString("dd MMM. dddd", new CultureInfo("es-ES"))
        //),
        //                Estado = empleadoHorasTrabajo.Estado,
        //                Dispositivo = empleadoHorasTrabajo.Dispositivo,
        //                Activo = true,
        //                TotalHorasReloj = input.TotalHorasReloj,
        //                TotalNormales = empleadoHorasTrabajo.TotalNormales,
        //                TotalDiurna = empleadoHorasTrabajo.TotalDiurna,
        //                TotalNocturna = empleadoHorasTrabajo.TotalNocturna,
        //                TotalMixta = empleadoHorasTrabajo.TotalMixta,
        //                TotalNoTrabajado = 0,
        //                Entrada = TimeSpan.TryParse(input.Entrada, out var entrada) ? entrada : (TimeSpan?)null,
        //                Salida = TimeSpan.TryParse(input.Salida, out var salida) ? salida : (TimeSpan?)null,
        //                Receso_Desde = TimeSpan.TryParse(input.RecesoDesde, out var recesoDesde) ? recesoDesde : (TimeSpan?)null,
        //                Receso_Hasta = TimeSpan.TryParse(input.RecesoHasta, out var recesoHasta) ? recesoHasta : (TimeSpan?)null,
        //                IdEmpleadoHorario = input.IdEmpleadoHorario ?? empleadoHorasTrabajo.IdEmpleadoHorario

        //            };


        //            SetCamposAuditoria(detalle, true, usuarioActual);
        //            _context.Add(detalle);
        //        }

        //        await _context.SaveChangesAsync();

        //        //return Json(new { success = true, message = "Registro creado exitosamente." });
        //        TempData["success"] = "El registro ha actualizado exitosamente.";
        //        return Json(new { success = true, redirectUrl = Url.Action("Index") });

        //    }
        //    catch (DbUpdateException dbEx)
        //    {
        //        return StatusCode(500, "Error al guardar en base de datos: " + (dbEx.InnerException?.Message ?? dbEx.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error inesperado: " + ex.Message);
        //    }
        //}

        public async Task<IActionResult> Create(EmpleadoHorasTrabajoViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.JsonRegistros))
                    return BadRequest("No se recibió información de registros (JsonRegistros está vacío o nulo).");

                var registrosInput = JsonConvert.DeserializeObject<List<EmpleadoHorasTrabajoInputModel>>(model.JsonRegistros);

                if (registrosInput == null || !registrosInput.Any())
                    return BadRequest("El JsonRegistros no contiene registros válidos.");

                var usuarioActual = _userManager.GetUserName(User);
                var fechaActual = DateTime.Now;

                foreach (var input in registrosInput)
                {
                    var detalle = new EmpleadoHorasTrabajo
                    {
                        IdEmpleado = input.IdEmpleado,
                        Fecha = fechaActual,
                        DiaSemana = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                            input.Fecha.ToString("dd MMM. dddd", new CultureInfo("es-ES"))
                        ),
                        Estado = model.Estado,
                        Dispositivo = model.Dispositivo,
                        Activo = true,
                        TotalHorasReloj = input.TotalHorasReloj,
                        TotalNormales = model.TotalNormales,
                        TotalDiurna = model.TotalDiurna,
                        TotalNocturna = model.TotalNocturna,
                        TotalMixta = model.TotalMixta,
                        TotalNoTrabajado = 0,
                        Entrada = TimeSpan.TryParse(input.Entrada, out var entrada) ? entrada : null,
                        Salida = TimeSpan.TryParse(input.Salida, out var salida) ? salida : null,
                        Receso_Desde = TimeSpan.TryParse(input.RecesoDesde, out var recesoDesde) ? recesoDesde : null,
                        Receso_Hasta = TimeSpan.TryParse(input.RecesoHasta, out var recesoHasta) ? recesoHasta : null,
                        IdEmpleadoHorario = input.IdEmpleadoHorario ?? model.IdEmpleadoHorario
                    };

                    SetCamposAuditoria(detalle, true, usuarioActual);
                    _context.Add(detalle);
                }

                await _context.SaveChangesAsync();

                TempData["success"] = "El registro ha actualizado exitosamente.";
                return Json(new { success = true, redirectUrl = Url.Action("Index") });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, "Error al guardar en base de datos: " + (dbEx.InnerException?.Message ?? dbEx.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error inesperado: " + ex.Message);
            }
        }

        // GET: EmpleadoHorasTrabajo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHorasTrabajo = await _context.EmpleadoHorasTrabajos.FindAsync(id);
            if (empleadoHorasTrabajo == null)
            {
                return NotFound();
            }
            return View(empleadoHorasTrabajo);
        }

        // POST: EmpleadoHorasTrabajo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleadoHorasTrabajo,IdEmpleado,Fecha,Entrada,Salida,Receso_Desde,Receso_Hasta,IdEmpleadoHorario,TotalNormales,TotalDiurna,TotalNocturna,TotalMixta,TotalNoTrabajado,Estado,Activo,FechaCreacion,FechaModificacion,CreadoPor,ModificadoPor,Dispositivo,DiaSemana,TotalHorasReloj")] EmpleadoHorasTrabajo empleadoHorasTrabajo)
        {
            if (id != empleadoHorasTrabajo.IdEmpleadoHorasTrabajo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioActual = _userManager.GetUserName(User);
                    SetCamposAuditoria(empleadoHorasTrabajo, false, usuarioActual);
                    _context.Update(empleadoHorasTrabajo);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "El registro ha actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoHorasTrabajoExists(empleadoHorasTrabajo.IdEmpleadoHorasTrabajo))
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
            return View(empleadoHorasTrabajo);
        }

        // GET: EmpleadoHorasTrabajo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleadoHorasTrabajo = await _context.EmpleadoHorasTrabajos
                .FirstOrDefaultAsync(m => m.IdEmpleadoHorasTrabajo == id);
            if (empleadoHorasTrabajo == null)
            {
                return NotFound();
            }

            return View(empleadoHorasTrabajo);
        }

        // POST: EmpleadoHorasTrabajo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             var empleadoHorasTrabajo = await _context.EmpleadoHorasTrabajos.FindAsync(id);
            try
            {
               
                if (empleadoHorasTrabajo != null)
                {
                    _context.EmpleadoHorasTrabajos.Remove(empleadoHorasTrabajo);
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
                return View(empleadoHorasTrabajo);
            }

        }

        private bool EmpleadoHorasTrabajoExists(int id)
        {
            return _context.EmpleadoHorasTrabajos.Any(e => e.IdEmpleadoHorasTrabajo == id);
        }

        private void SetCamposAuditoria(EmpleadoHorasTrabajo record, bool bNewRecord, string currentUser)
        {
            var now = DateTime.Now;

            if (bNewRecord)
            {
                record.FechaCreacion = now;
                record.CreadoPor = currentUser;
                record.FechaModificacion = now;
                record.ModificadoPor = currentUser;
                record.Activo = true;
            }
            else
            {
                record.FechaModificacion = now;
                record.ModificadoPor = currentUser;
            }
        }

    }
}
