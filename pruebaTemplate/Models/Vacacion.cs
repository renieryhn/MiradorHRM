using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

  
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

  
    [Display(Name = "Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

   
    [Display(Name = "Creado Por")]
    public string CreadoPor { get; set; } = null!;

 
    [Display(Name = "Modificado Por")] 
    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    [Display(Name = "Detalles de Vacación")]
    public virtual ICollection<VacacionDetalle> VacacionDetalles { get; set; } = new List<VacacionDetalle>();

    [NotMapped]
    public int DiasTotales { get; internal set; }
}
