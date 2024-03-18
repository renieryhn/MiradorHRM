using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class Deduccion
{
    public string IdDeduccion { get; set; } = null!;
    [DisplayName("Nombre de Deducción")]
    public string NombreDeduccion { get; set; } = null!;

    /// <summary>
    /// Fijo, Fórmula o Porcentaje
    /// </summary>
    [DisplayName("Tipo")]
    public string Tipo { get; set; } = null!;

    [DisplayName("Monto")]
    public decimal? Monto { get; set; }

    [DisplayName("Fórmula")]
    public string? Formula { get; set; }

    [DisplayName("Deducible de Impuesto")]
    public bool DeducibleImpuesto { get; set; }

    [DisplayName("Basada en todo")]
    public bool BasadoEnTodo { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<EmpleadoDeduccion> EmpleadoDeduccions { get; set; } = new List<EmpleadoDeduccion>();
    
    //public  TipoDeduccion = Enum.GetValues<TipoDeduccion>().ToList();

    public enum TipoDeduccion
    {
        Fijo,
        Fórmula,
        Porcentaje
    }

}
