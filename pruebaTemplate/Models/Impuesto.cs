using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Impuesto
{
    public string IdImpuesto { get; set; } = null!;
    [Display(Name = "Nombre de Impuesto")]
    public string NombreImpuesto { get; set; } = null!;

    /// <summary>
    /// Fijo, Fórmula, Porcentaje o Tabla
    /// </summary>
    [Display(Name = "Tipo de Impuesto")]
    public string Tipo { get; set; } = null!;
    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }
    [Display(Name = "Fórmula")]
    public string? Formula { get; set; }
    [Display(Name = "Es Grabable")]
    public bool Grabable { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public enum TipoImpuesto
    {
        Fijo,
        Fórmula,
        Porcentaje
    }
}
