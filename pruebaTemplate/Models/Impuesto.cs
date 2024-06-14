using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Impuesto
{
    [Display(Name = "ID del Impuesto")]
    [Required(ErrorMessage = "El ID del impuesto es requerido")]
    public int IdImpuesto { get; set; }

    [Display(Name = "Nombre del Impuesto")]
    [Required(ErrorMessage = "El nombre del impuesto es requerido")]
    public string NombreImpuesto { get; set; } = null!;

    [Display(Name = "Tipo de Impuesto")]
    [Required(ErrorMessage = "El tipo de impuesto es requerido")]
    public TipoImpuesto Tipo { get; set; }

    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }

    [Display(Name = "Fórmula")]
    public string? Formula { get; set; }

    [Display(Name = "Orden")]
    [Required(ErrorMessage = "El orden es requerido")]
    public int Orden { get; set; }

    [Display(Name = "Asignación Automática")]
    [Required(ErrorMessage = "El campo de asignación automática es requerido")]
    public bool AsignacionAutomatica { get; set; }

    [Display(Name = "Activo")]
    [Required(ErrorMessage = "El campo activo es requerido")]
    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;
    
    public virtual ICollection<EmpleadoImpuesto> EmpleadoImpuestos { get; set; } = new List<EmpleadoImpuesto>();
   
    public virtual ICollection<NominaImpuesto> NominaImpuestos { get; set; } = new List<NominaImpuesto>();

    public enum TipoImpuesto
    {
        Fijo,
        Fórmula,
        Porcentaje,
        Tabla
    }
}
