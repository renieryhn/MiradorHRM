using PlanillaPM.Models;

namespace MiradorHRM.Models
{
    public class EmpleadoHorasTrabajoDetalleViewModel
    {
        public EmpleadoHorasTrabajo RegistroActual { get; set; } = null!;
        public List<EmpleadoHorasTrabajo> RegistrosDelMes { get; set; } = new();
    }
}
