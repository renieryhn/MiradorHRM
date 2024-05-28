using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Impuesto
{
    public int IdImpuesto { get; set; } 
    [Display(Name = "Nombre de Impuesto")]
    public string NombreImpuesto { get; set; } = null!;

    [Display(Name = "Tipo de Impuesto")]
    public TipoImpuesto Tipo { get; set; }

    [Display(Name = "Valor")]
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
        Porcentaje,
        Tabla
    }
}
