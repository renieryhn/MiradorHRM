namespace MiradorHRM.Models
{
    public class RegistroEmpleadoViewModel
    {

        public int IdEmpleado { get; set; }
        public int IdEmpleadoHorario { get; set; }
        public string Nombre { get; set; }
        public string Dia { get; set; } // 26 May. Lunes
        public DateTime Fecha { get; set; }
        public string Entrada { get; set; }
        public string Salida { get; set; }
        public string RecesoDesde { get; set; }
        public string RecesoHasta { get; set; }
        public string Horas { get; set; }

        public string Dispositivo { get; set; } = "---";

        public string TipoJornada { get; set; }
        public string Comentario { get; set; }

        public string MotivoCaptura { get; set; } = "Reloj";

    }
}
