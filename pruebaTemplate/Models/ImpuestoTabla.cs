using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class ImpuestoTabla
{
    public int IdImpuestoTabla { get; set; }

    public string IdImpuesto { get; set; } = null!;

    public decimal Desde { get; set; }

    public decimal Hasta { get; set; }

    public decimal? Monto { get; set; }

    public decimal? Porcentaje { get; set; }
}
