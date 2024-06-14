using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoImpuesto
{
    [Display(Name = "ID del Empleado Impuesto")]
    public int IdEmpleadoImpuesto { get; set; }

    [Display(Name = "ID del Impuesto")]
    [Required(ErrorMessage = "El ID del impuesto es requerido")]
    public int IdImpuesto { get; set; }

    [Display(Name = "ID del Empleado")]
    [Required(ErrorMessage = "El ID del empleado es requerido")]
    public int IdEmpleado { get; set; }

    [Display(Name = "Exento")]
    [Required(ErrorMessage = "El campo Exento es requerido")]
    public bool Excento { get; set; }

    [Display(Name = "Orden")]
    [Required(ErrorMessage = "El orden es requerido")]
    public int Orden { get; set; }

    public DateTime FechaCreacion { get; set; }

    [Display(Name = "Activo")]
    [Required(ErrorMessage = "El campo Activo es requerido")]
    public bool Activo { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    [Display(Name = "Impuesto")]
    public virtual Impuesto IdImpuestoNavigation { get; set; } = null!;
}
