using PlanillaPM.ViewModel;
using static PlanillaPM.Models.EmpleadoAusencium;

namespace PlanillaPM.Models
{
    public class InicioFiltros
    {
        public ProfileViewModel Profile { get; set; }
        public int CantidadEmpleados { get; set; }
        public int CantidadUsuarios { get; set; }
        public int CantidadPerfilesCompletos { get; set; }
        public int CantidadCargos { get; set; }
        public List<Empleado> ProximosCumpleañeros { get; set; }
        public List<Empleado> LicenciasPorVencer { get; set; }
        public List<EmpleadoContrato> ContratosPorVencer { get; set; }
        public List<EmpleadoAusencium> EmpleadoAusencias { get; set; }

        public int Id { get; set; }
        public EstadoAusencia NuevoEstado { get; set; }
       
        
    }
}
