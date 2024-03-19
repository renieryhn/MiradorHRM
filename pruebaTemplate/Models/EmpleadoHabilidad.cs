using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoHabilidad
{
    public int IdEmpleadoHabilidad { get; set; }

    [Display(Name = "Id Empleado")]
    public int IdEmpleado { get; set; }

    [Required(ErrorMessage = "El campo Habilidad es obligatorio.")]
    [Display(Name = "Habilidad")]
    public string Habilidad { get; set; } = null!;

    [Display(Name = "Años de Experiencia")]
    public int ExperienciaYears { get; set; }

    [Display(Name = "Comentarios")]
    public string? Comentarios { get; set; }

    [DisplayName("Activo")]
    public bool Activo { get; set; }

    [Display(Name = "Fecha Creación")]
    public DateTime FechaCreacion { get; set; }

    [Display(Name = "Fecha Modificación")]
    public DateTime FechaModificacion { get; set; }

    [Display(Name = "Creado Por")]
    public string CreadoPor { get; set; } = null!;

    [Display(Name = "Modificado Por")]
    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Id Empleado Navigation")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
