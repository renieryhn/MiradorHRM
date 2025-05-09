using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanillaPM.Models;

namespace PlanillaPM.Controllers
{
   
    public class ConstanciaController : Controller
    {

        private readonly PlanillaContext _context;

        public ConstanciaController(PlanillaContext context)
        {
            _context = context;
        }
        // GET: ConstaciaController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Constancia1(int id)
        {
            // Obtener los datos del empleado correspondiente al ID
            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound(); 
            }

            

            ViewData["Nombre"] = empleado.NombreCompleto;
            ViewData["Identidad"] = empleado.NumeroIdentidad;

            return View();
        }

        public IActionResult Constancia2(int id)
        {
            // Obtener los datos del empleado correspondiente al ID
            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }

          
            ViewData["Nombre"] = empleado.NombreCompleto;
            ViewData["Identidad"] = empleado.NumeroIdentidad;
            return View();
        }

        public IActionResult ConstanciaTrabajo(int id)
        {
            // Obtener los datos del empleado correspondiente al ID
            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            ViewData["Nombre"] = empleado.NombreCompleto;
            ViewData["Identidad"] = empleado.NumeroIdentidad;
            ViewData["SalarioBase"] = empleado.SalarioBase;

            return View();
        }

        public IActionResult FContrato(int id)
        {
            // Obtener los datos del empleado correspondiente al ID
            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            ViewData["Nombre"] = empleado.NombreCompleto;
            ViewData["Identidad"] = empleado.NumeroIdentidad;
            ViewData["SalarioBase"] = empleado.SalarioBase;

            return View();
        }

        public IActionResult FContratoCorto(int id)
        {
            // Obtener los datos del empleado correspondiente al ID
            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);
            if (empleado == null)
            {
                return NotFound();
            }


            ViewData["Nombre"] = empleado.NombreCompleto;
            ViewData["Identidad"] = empleado.NumeroIdentidad;
            ViewData["SalarioBase"] = empleado.SalarioBase;

            return View();
        }
        
       
    }
}
