using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class ImpuestoTabla
{
    public int IdImpuestoTabla { get; set; }
    [Display(Name = "Impuesto")]
    public string IdImpuesto { get; set; } = null!;
    [Display(Name = "Desde")]
    public decimal Desde { get; set; }
    [Display(Name = "Hasta")]
    public decimal Hasta { get; set; }
    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }
    [Display(Name = "Porcentaje")]
    public decimal? Porcentaje { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
}
