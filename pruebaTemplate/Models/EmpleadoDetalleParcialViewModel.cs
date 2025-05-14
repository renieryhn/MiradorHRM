using PlanillaPM.Models;

namespace MiradorHRM.Models
{
    public class EmpleadoDetalleParcialViewModel
    {
        public int IdEmpleado { get; set; }

        // Puedes usar estas como opcionales, según el partial
        public List<EmpleadoIngreso>? Ingresos { get; set; }
        public List<EmpleadoDeduccion>? Deducciones { get; set; }
        public List<EmpleadoImpuesto>? Impuestos { get; set; }

        // Auxiliares para combos si necesitas
        public List<Ingreso>? ListaIngresos { get; set; }
        public List<Deduccion>? ListaDeducciones { get; set; }
        public List<Impuesto>? ListaImpuestos { get; set; }
    }
}
