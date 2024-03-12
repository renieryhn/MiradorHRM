using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoAusencium
{
    public int IdEmpleadoAusencia { get; set; }

    public int IdEmpleado { get; set; }

    public int IdTipoAusencia { get; set; }

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

    public string? Comentarios { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual TipoAusencium IdTipoAusenciaNavigation { get; set; } = null!;
}
