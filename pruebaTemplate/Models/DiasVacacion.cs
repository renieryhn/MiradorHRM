using System.ComponentModel.DataAnnotations;

namespace MiradorHRM.Models
{
    public class DiasVacacion
    {
        public int IdDiaVacion { get; set; }
        public int Hasta { get; set; }
        public int DiasVacaciones { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }
    }

}
