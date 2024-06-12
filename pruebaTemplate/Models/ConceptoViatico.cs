using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class ConceptoViatico
{
    public int IdConceptoViatico { get; set; }

    public string NombreConceptoViatico { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<ViaticoDetalle> ViaticoDetalles { get; set; } = new List<ViaticoDetalle>();
}
