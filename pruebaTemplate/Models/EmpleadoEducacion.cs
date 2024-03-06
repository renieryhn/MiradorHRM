using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoEducacion
{
    public int IdEmpleadoEducacion { get; set; }

    public int IdEmpleado { get; set; }

    public string Institucion { get; set; } = null!;

    public string TituloObtenido { get; set; } = null!;

    public DateOnly FechaDesde { get; set; }

    public DateOnly FechaHasta { get; set; }

    public string? Comentarios { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
