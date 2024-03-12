using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoEducacion
{
    public int IdEmpleadoEducacion { get; set; }

    [Display(Name = "Id Empleado")]
    public int IdEmpleado { get; set; }

    [Display(Name = "Institución")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Institucion { get; set; } = null!;

    [Display(Name = "Título Obtenido")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string TituloObtenido { get; set; } = null!;

    [Display(Name = "Fecha Desde")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public DateOnly FechaDesde { get; set; }

    [Display(Name = "Fecha Hasta")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public DateOnly FechaHasta { get; set; }

    public string? Comentarios { get; set; }

    [Display(Name = "Fecha Creación")]
    public DateOnly FechaCreacion { get; set; }

    [Display(Name = "Fecha Modificación")]
    public DateOnly FechaModificacion { get; set; }

    [Display(Name = "Creado Por")]
    public string CreadoPor { get; set; } = null!;

    [Display(Name = "Modificado Por")]
    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
