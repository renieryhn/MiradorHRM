using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Vacacion
{
    [Key]
    public int IdVacacion { get; set; }

    [Required(ErrorMessage = "El ID del empleado es requerido")]
    [Display(Name = "ID del Empleado")]
    public int IdEmpleado { get; set; }

    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    [Required(ErrorMessage = "El periodo vacacional es requerido")]
    [Display(Name = "Periodo Vacacional")]
    public int PeriodoVacacional { get; set; }

    [Required(ErrorMessage = "El total de días del periodo es requerido")]
    [Display(Name = "Total de Días del Periodo")]
    public int TotalDiasPeriodo { get; set; }

    [Required(ErrorMessage = "Los días gozados son requeridos")]
    [Display(Name = "Días Gozados")]
    public int DiasGozados { get; set; }

    [Required(ErrorMessage = "Los días pendientes son requeridos")]
    [Display(Name = "Días Pendientes")]
    public int DiasPendientes { get; set; }

    [Required(ErrorMessage = "El estado activo es requerido")]
    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    [Required(ErrorMessage = "La fecha de creación es requerida")]
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [Required(ErrorMessage = "La fecha de modificación es requerida")]
    [Display(Name = "Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [Required(ErrorMessage = "El creador es requerido")]
    [Display(Name = "Creado Por")]
    [StringLength(50, ErrorMessage = "El nombre del creador no puede tener más de 50 caracteres")]
    public string CreadoPor { get; set; } = null!;

    [Required(ErrorMessage = "El modificador es requerido")]
    [Display(Name = "Modificado Por")]
    [StringLength(50, ErrorMessage = "El nombre del modificador no puede tener más de 50 caracteres")]
    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    [Display(Name = "Detalles de Vacación")]
    public virtual ICollection<VacacionDetalle> VacacionDetalles { get; set; } = new List<VacacionDetalle>();
}
