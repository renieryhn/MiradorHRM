using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class ImpuestoTabla
{
    [Display(Name = "Id")]
    public int IdImpuestoTabla { get; set; }
    [Display(Name = "Impuesto")]
    public int IdImpuesto { get; set; } 
    [Display(Name = "Desde")]
    public decimal Desde { get; set; }
    [Display(Name = "Hasta")]
    public decimal Hasta { get; set; }
    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }
    [Display(Name = "Porcentaje")]
    public decimal? Porcentaje { get; set; }

    [DisplayName("Activo")]
    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
}
