using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class DeduccionIngreso
{
    public int IdDeduccionIngreso { get; set; }

    public int IdDeduccion { get; set; }

    public int IdIngreso { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Deduccion IdDeduccionNavigation { get; set; } = null!;

    public virtual Ingreso IdIngresoNavigation { get; set; } = null!;
}
