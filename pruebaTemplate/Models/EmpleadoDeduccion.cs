using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoDeduccion
{
    public string IdDeduccion { get; set; } = null!;

    public int IdEmpleado { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Deduccion IdDeduccionNavigation { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}

