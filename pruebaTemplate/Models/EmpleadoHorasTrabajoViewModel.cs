using PlanillaPM.Models;

namespace MiradorHRM.Models
{

    public class EmpleadoHorasTrabajoViewModel
    {
        public string JsonRegistros { get; set; }

        public string Dispositivo { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public bool Activo { get; set; } = true;

        public TimeSpan? TotalNormales { get; set; }
        public decimal TotalDiurna { get; set; }
        public decimal TotalNocturna { get; set; }
        public decimal TotalMixta { get; set; }

        // Solo si lo necesitas explícitamente
        public int IdEmpleadoHorario { get; set; }
    }

    //public class EmpleadoHorasTrabajoViewModel
    //{
    //    public string JsonRegistros { get; set; }
    //    public DateTime Fecha { get; set; }
    //    public string Estado { get; set; }
    //    public bool Activo { get; set; }
    //    public string Dispositivo { get; set; }
    //    public decimal TotalNormales { get; set; }
    //    public decimal TotalDiurna { get; set; }
    //    public decimal TotalNocturna { get; set; }
    //    public decimal TotalMixta { get; set; }

    //    public EmpleadoHorasTrabajo Encabezado { get; set; }
    //    public List<EmpleadoHorasTrabajo> Detalle { get; set; }
    //}

    public class EmpleadoHorasTrabajoInputModel
    {
        public int IdEmpleado { get; set; }
        public DateTime Fecha { get; set; }
        public string DiaSemana { get; set; }
        public string Entrada { get; set; }
        public string Salida { get; set; }
        public string RecesoDesde { get; set; }
        public string RecesoHasta { get; set; }
        public int? IdEmpleadoHorario { get; set; }

        public TimeSpan? TotalHorasReloj { get; set; }
    }

}
