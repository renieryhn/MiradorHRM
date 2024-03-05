using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoDeduccion
{
    public string IdDeduccion { get; set; } = null!;

    public int IdEmpleado { get; set; }

    public virtual Deduccion IdDeduccionNavigation { get; set; } = null!;
}
