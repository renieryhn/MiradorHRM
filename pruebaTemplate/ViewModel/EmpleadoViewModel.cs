using PlanillaPM.Models;

namespace PlanillaPM.ViewModel
{
    public class EmpleadoViewModel
    {
        public List<Empleado> Empleados { get; set; }
        public Empleado? EmpleadoSeleccionado { get; set; }
    }
}
