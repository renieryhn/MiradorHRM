using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoHabilidad
{
    public int IdEmpleadoHabilidad { get; set; }

    public int IdEmpleado { get; set; }

    [Required(ErrorMessage = "El campo Habilidad es obligatorio.")]
    public string Habilidad { get; set; } = null!;

    [Required(ErrorMessage = "El campo Años de Experiencia es obligatorio.")]
    public int ExperienciaYears { get; set; }

    public string? Comentarios { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
