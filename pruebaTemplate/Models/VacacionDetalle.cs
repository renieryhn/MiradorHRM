using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class VacacionDetalle
{
    [Key]
    public int IdVacacionDetalle { get; set; }

    [Required(ErrorMessage = "El ID de la vacación es obligatorio.")]
    [Display(Name = "ID de Vacación")]
    public int IdVacacion { get; set; }

    [Required(ErrorMessage = "El ID del empleado es obligatorio.")]
    [Display(Name = "ID del Empleado")]
    public int IdEmpleado { get; set; }

    [Required(ErrorMessage = "La fecha de solicitud es obligatoria.")]
    [Display(Name = "Fecha de Solicitud")]
    public DateOnly FechaSolicitud { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [Display(Name = "Fecha de Inicio")]
    public DateOnly FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    [Display(Name = "Fecha de Fin")]
    public DateOnly FechaFin { get; set; }

    [Required(ErrorMessage = "El número de días solicitados es obligatorio.")]
    [Display(Name = "Número de Días Solicitados")]
    public int NumeroDiasSolicitados { get; set; }

    [Required(ErrorMessage = "El estado de la solicitud es obligatorio.")]
    [Display(Name = "Estado de la Solicitud")]
    public Estado EstadoSolicitud { get; set; }

    [Display(Name = "Aprobado Por")]
    public string? AprobadoPor { get; set; }

    [Required(ErrorMessage = "El número de días aprobados es obligatorio.")]
    [Display(Name = "Días Aprobados")]
    public int DiasAprobados { get; set; }

    [Display(Name = "Comentarios del Aprobador")]
    public string? ComentariosAprobador { get; set; }

    [Required(ErrorMessage = "El campo activo es obligatorio.")]
    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    [Display(Name = "Vacacion")]
    public virtual Vacacion IdVacacionNavigation { get; set; } = null!;

    public enum Estado
    {
        Pendiente = 1,
        Aprobada = 2,
        Rechazada = 3
    }
}
