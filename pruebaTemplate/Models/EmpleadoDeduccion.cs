using PlanillaPM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoDeduccion
{
    [Display(Name = "ID Empleado Deducción")]
    [Required(ErrorMessage = "El ID de empleado deducción es requerido")]
    public int IdEmpleadoDeduccion { get; set; }

    [Display(Name = "ID Empleado")]
    [Required(ErrorMessage = "El ID de empleado es requerido")]
    public int IdEmpleado { get; set; }

    [Display(Name = "ID Deducción")]
    [Required(ErrorMessage = "El ID de deducción es requerido")]
    public int IdDeduccion { get; set; }

    /// <summary>
    /// Fijo, Fórmula o Porcentaje
    /// </summary>
    [Display(Name = "Tipo")]
    [Required(ErrorMessage = "El tipo es requerido")]
    public TipoEstado Tipo { get; set; }

    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }

    [Display(Name = "Fórmula")]
    public string? Formula { get; set; }

    [Display(Name = "Orden")]
    [Required(ErrorMessage = "El orden es requerido")]
    public int Orden { get; set; }

    [Display(Name = "Activo")]
    [Required(ErrorMessage = "El estado activo es requerido")]
    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Deduccion")]
    public virtual Deduccion IdDeduccionNavigation { get; set; } = null!;

    [Display(Name = "Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public enum TipoEstado
    {
        Fijo = 1,
        Fórmula = 2,
        Porcentaje = 3
    }
}
