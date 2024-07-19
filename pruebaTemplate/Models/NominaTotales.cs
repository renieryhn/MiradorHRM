using MiradorHRM.Models;
using static PlanillaPM.Models.Viatico;

namespace PlanillaPM.Models
{
    public class NominaTotales
    {
        public decimal TotalIngresos { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalImpuestos { get; set; }
        public int TotalEmpleadosEnNomina { get; set; }
        public int DiasAprobados { get; set; }
        public string ComentariosAprobador { get; set; }
        public string AprobadoPor { get; set; }
        public int Id { get; set; }
        public TipoEstado EstadoViatico { get; set; }
        public int IdNomina { get; set; }
        public int PeriodoFiscal { get; set; }
        public int Mes { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal PagoNeto { get; set; }
        public int EstadoNomina { get; set; }

        public List<VacacionDetalle> VacacionDetalle { get; set; }
        public List<Nomina> NominaAprovacion { get; set; }
        public List<NominaDataCharts> NominaData { get; set; }
        public List<Viatico> Viatico { get; set; }
    }
}
