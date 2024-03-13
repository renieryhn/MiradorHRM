using System;
using System.Collections.Generic;
using System.ComponentModel;

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

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<EmpleadoIngreso> EmpleadoIngresos { get; set; } = new List<EmpleadoIngreso>();
}
