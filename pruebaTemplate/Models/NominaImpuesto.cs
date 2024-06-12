using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class NominaImpuesto
{
    public int IdNominaImpuesto { get; set; }

    public int IdNominaDetalle { get; set; }

    public int IdImpuesto { get; set; }

    public decimal Monto { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Impuesto IdImpuestoNavigation { get; set; } = null!;

    public virtual NominaDetalle IdNominaDetalleNavigation { get; set; } = null!;
}
