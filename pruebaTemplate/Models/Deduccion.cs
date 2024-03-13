using System;
using System.Collections.Generic;
using System.ComponentModel;

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

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<EmpleadoDeduccion> EmpleadoDeduccions { get; set; } = new List<EmpleadoDeduccion>();
}
