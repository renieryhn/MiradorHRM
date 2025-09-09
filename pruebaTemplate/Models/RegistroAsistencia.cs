namespace MiradorHRM.Models
{
    public class RegistroAsistencia
    {
        public int IdEmpleado { get; set; }
        public string NombreEmpleado { get; set; } = "";
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; } = "";
    }
}
