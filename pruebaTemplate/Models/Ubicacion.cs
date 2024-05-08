using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PlanillaPM.Models
{
    public class Ubicacion
    {
        public int IdUbicacion { get; set; }

        [Display(Name = "Ubicación")]
        [Required(ErrorMessage = "El nombre de la ubicación es obligatorio.")]
        public string NombreUbicacion { get; set; } = null!;

        [Display(Name = "Ciudad")]
        public string? Ciudad { get; set; }

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El Teléfono es obligatorio.")]
        public string Telefono { get; set; } = null!;

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Fecha de Modificación")]
        public DateTime FechaModificacion { get; set; }

        [DisplayName("Creado Por")]
        public string? CreadoPor { get; set; }

        [DisplayName("Modificado Por")]
        public string? ModificadoPor { get; set; }

        public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
