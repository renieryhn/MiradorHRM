using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class Ingreso
{
    public string IdIngreso { get; set; } = null!;

    public string NombreIngreso { get; set; } = null!;

    /// <summary>
    /// Fijo, Fórmula o Porcentaje
    /// </summary>
    public string Tipo { get; set; } = null!;

    public decimal? Monto { get; set; }

    public string? Formula { get; set; }

    public bool Grabable { get; set; }

    public int Orden { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<EmpleadoIngreso> EmpleadoIngresos { get; set; } = new List<EmpleadoIngreso>();
}
