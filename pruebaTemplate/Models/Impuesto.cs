using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class Impuesto
{
    public string IdImpuesto { get; set; } = null!;

    public string NombreImpuesto { get; set; } = null!;

    /// <summary>
    /// Fijo, Fórmula, Porcentaje o Tabla
    /// </summary>
    public string Tipo { get; set; } = null!;

    public decimal? Monto { get; set; }

    public string? Formula { get; set; }

    public bool Grabable { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;
}
