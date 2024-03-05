using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class Deduccion
{
    public string IdDeduccion { get; set; } = null!;

    public string NombreDeduccion { get; set; } = null!;

    /// <summary>
    /// Fijo, Fórmula o Porcentaje
    /// </summary>
    public string Tipo { get; set; } = null!;

    public decimal? Monto { get; set; }

    public string? Formula { get; set; }

    public bool DeducibleImpuesto { get; set; }

    public bool BasadoEnTodo { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<EmpleadoDeduccion> EmpleadoDeduccions { get; set; } = new List<EmpleadoDeduccion>();
}
