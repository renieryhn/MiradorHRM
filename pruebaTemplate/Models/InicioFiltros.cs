using PlanillaPM.ViewModel;

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
    }
}
