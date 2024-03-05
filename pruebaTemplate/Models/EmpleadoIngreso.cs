using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoIngreso
{
    public string IdIngreso { get; set; } = null!;

    public int IdEmpleado { get; set; }

    public virtual Ingreso IdIngresoNavigation { get; set; } = null!;
}
