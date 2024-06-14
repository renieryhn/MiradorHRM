using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class NominaDeduccion
{
    public int IdNominaDeduccion { get; set; }

    public int IdNominaDetalle { get; set; }

    public int IdDeduccion { get; set; }

    public decimal Monto { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Deduccion IdDeduccionNavigation { get; set; } = null!;

    public virtual NominaDetalle IdNominaDetalleNavigation { get; set; } = null!;
}
