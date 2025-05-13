using PlanillaPM.Models;

namespace PlanillaPM.ViewModel
{
    public class EmpleadoViewModel
    {
        public List<Empleado> Empleados { get; set; }
        public Empleado? EmpleadoSeleccionado { get; set; }
        public List<EmpleadoIngreso> Ingresos { get; set; }
        public List<Empleado> EmpleadosActivos { get; set; }
    }
}
