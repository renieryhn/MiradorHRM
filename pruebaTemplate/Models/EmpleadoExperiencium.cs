using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoExperiencium
{
    public int IdEmpleadoExperiencia { get; set; }

    public int IdEmpleado { get; set; }

    public string Empresa { get; set; } = null!;

    public string Cargo { get; set; } = null!;

    public DateOnly FechaDesde { get; set; }

    public DateOnly FechaHasta { get; set; }

    public string? Descripcion { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
