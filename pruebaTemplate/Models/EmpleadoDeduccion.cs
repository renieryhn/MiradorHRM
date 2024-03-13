using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class EmpleadoDeduccion
{
    public string IdDeduccion { get; set; } = null!;

    public int IdEmpleado { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual Deduccion IdDeduccionNavigation { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}

