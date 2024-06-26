using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoIngreso
{
    [Key]
    public int IdEmpleadoIngreso { get; set; }

    [Display(Name = "Ingreso")]
    [Required(ErrorMessage = "El Id de Ingreso es requerido")]
    public int IdIngreso { get; set; }

    [Display(Name = "Empleado")]
    [Required(ErrorMessage = "El Id de Empleado es requerido")]
    public int IdEmpleado { get; set; }

    [Display(Name = "Tipo")]
    [Required(ErrorMessage = "El tipo es requerido")]
    public TipoEstado Tipo { get; set; }

    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }

    [Display(Name = "Fórmula")]
    public string? Formula { get; set; }

    [Display(Name = "Orden")]
    public int Orden { get; set; }

    [Display(Name = "Activo")]
    [Required(ErrorMessage = "El campo Activo es requerido")]
    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;
    [Display(Name = "Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
    [Display(Name = "Ingreso")]
    public virtual Ingreso IdIngresoNavigation { get; set; } = null!;
    public enum TipoEstado
    {
        Fijo = 1,
        Fórmula = 2,
        Porcentaje = 3
    }
}
