using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoHabilidad
{
    public int IdEmpleadoHabilidad { get; set; }

    public int IdEmpleado { get; set; }

    public string Habilidad { get; set; } = null!;

    public int ExperienciaYears { get; set; }

    public string? Comentarios { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
