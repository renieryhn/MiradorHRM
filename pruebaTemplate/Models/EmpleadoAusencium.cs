using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoAusencium
{
    
    public int IdEmpleadoAusencia { get; set; }

    [Display(Name = "Id Empleado")]
    public int IdEmpleado { get; set; }

    [Display(Name = "Id Tipo Ausencia")]
    public int IdTipoAusencia { get; set; }

    [Display(Name = "Dia Completo")]
    public bool DiaCompleto { get; set; }

    /// <summary>
    /// Solicitada/Aprobada/Rechazada
    /// </summary>
    [Required(ErrorMessage = "El campo Estado es obligatorio.")]
    public int Estado { get; set; }

    [Display(Name = "Fecha Desde")]
    [Required(ErrorMessage = "El campo Fecha Desde es obligatorio.")]
    public DateOnly FechaDesde { get; set; }

    [Display(Name = "Fecha Hasta")]
    [Required(ErrorMessage = "El campo Fecha Hasta es obligatorio.")]
    public DateOnly FechaHasta { get; set; }

    [Display(Name = "Hora Desde")]
    [Required(ErrorMessage = "El campo Hora Desde es obligatorio.")]
    public TimeOnly? HoraDesde { get; set; }

    [Display(Name = "Hora Hasta")]
    [Required(ErrorMessage = "El campo Hora Hasta es obligatorio.")]
    public TimeOnly? HoraHasta { get; set; }

    [Display(Name = "Aprobado Por")] 
    [Required(ErrorMessage = "El campo Aprobado Por es obligatorio.")]
    public string? AprobadoPor { get; set; }

    [Display(Name = "Comentarios")]
    public string? Comentarios { get; set; }

    [Display(Name = "Fecha Creación")]
    public DateOnly FechaCreacion { get; set; }

    [Display(Name = "Fecha Modificación")]
    public DateOnly FechaModificacion { get; set; }
    [Display(Name = "Creado Por")]
    public string CreadoPor { get; set; } = null!;
    [Display(Name = "Modificado Por")]
    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Id Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
    [Display(Name = "Tipo de Ausencia")]
    public virtual TipoAusencium IdTipoAusenciaNavigation { get; set; } = null!;
}
