using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoIngreso
{
    public string IdIngreso { get; set; } = null!;

    public int IdEmpleado { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Ingreso IdIngresoNavigation { get; set; } = null!;
}