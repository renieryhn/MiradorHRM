using MiradorHRM.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models
{
    public class Ventana
    {
        public int Id { get; set; }
        [DisplayName("Nombre")]
        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }

        // Relación con RoleVentana
        public virtual ICollection<RoleVentana> RoleVentanas { get; set; }
    }
}
