using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoCargoHistorico
{
    public int IdEmpleadoCargo { get; set; }

    public int IdEmpleado { get; set; }

    public int IdCargo { get; set; }

    public string? Comentario { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
