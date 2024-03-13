using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class ImpuestoTabla
{
    public int IdImpuestoTabla { get; set; }

    public string IdImpuesto { get; set; } = null!;

    public decimal Desde { get; set; }

    public decimal Hasta { get; set; }

    public decimal? Monto { get; set; }

    public decimal? Porcentaje { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
}
