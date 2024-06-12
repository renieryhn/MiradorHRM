using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class ViaticoDetalle
{
    public int IdViaticoDetalle { get; set; }

    public int IdViatico { get; set; }

    public int IdConceptoViatico { get; set; }

    public DateOnly FechaFatura { get; set; }

    public decimal Monto { get; set; }

    public string? FacturaPath { get; set; }

    public string? Comentarios { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ConceptoViatico IdConceptoViaticoNavigation { get; set; } = null!;

    public virtual Viatico IdViaticoNavigation { get; set; } = null!;
}
