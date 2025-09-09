using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanillaPM.Models
{
    public class EmpleadoHorasTrabajo
    {
        [Key]
        public int IdEmpleadoHorasTrabajo { get; set; }

        [DisplayName("Empleado")]
       
        public int IdEmpleado { get; set; }

        //[Display(Name = "Horario Base")]
        //public int? IdHorarioBase { get; set; }

        [DisplayName("Fecha")]
        [Required(ErrorMessage = "La Fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [DisplayName("Entrada")]
        public TimeSpan? Entrada { get; set; }

        [DisplayName("Salida")]
        public TimeSpan? Salida { get; set; }

        [DisplayName("Receso Desde")]
        public TimeSpan? Receso_Desde { get; set; }

        [DisplayName("Receso Hasta")]
        public TimeSpan? Receso_Hasta { get; set; }

        [DisplayName("Horas Normales")]
        public TimeSpan? TotalNormales { get; set; }

        [DisplayName("Horas Diurnas")]
        [Range(0, double.MaxValue)]
        public decimal TotalDiurna { get; set; }

        [DisplayName("Horas Nocturnas")]
        [Range(0, double.MaxValue)]
        public decimal TotalNocturna { get; set; }

        [DisplayName("Horas Mixtas")]
        [Range(0, double.MaxValue)]
        public decimal TotalMixta { get; set; }

        [DisplayName("No Trabajado")]
        [Range(0, double.MaxValue)]
        public decimal TotalNoTrabajado { get; set; }

        [DisplayName("Estado")]
        [StringLength(50)]
        public string? Estado { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }

        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Fecha de Modificación")]
        public DateTime FechaModificacion { get; set; }

        [DisplayName("Creado Por")] 
        public string CreadoPor { get; set; } = null!;

        [DisplayName("Modificado Por")]        
        public string ModificadoPor { get; set; } = null!;

        [DisplayName("Dispositivo")]
        [Required, StringLength(50)]
        public string Dispositivo { get; set; } = null!;

        [DisplayName("Empleado")]
        public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

        //[DisplayName("Horaio Base")]
        //public virtual Horario IdHorarioBaseNavigation { get; set; } = null!;

        [DisplayName("Día de la Semana")]
        [StringLength(20)]
        public string? DiaSemana { get; set; }


        [DisplayName("Horario Asignado")]
        public int? IdEmpleadoHorario { get; set; }

        [ForeignKey(nameof(IdEmpleadoHorario))]
        public virtual EmpleadoHorario? IdEmpleadoHorarioNavigation { get; set; }

        [DisplayName("Horas Reloj")]
        public TimeSpan? TotalHorasReloj { get; set; }

       
    }



}
