using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoExperiencium
{
    public int IdEmpleadoExperiencia { get; set; }

    [Display(Name = "Empleado")]
    [Required(ErrorMessage = "El campo Empleado es obligatorio.")]
    public int IdEmpleado { get; set; }

    [Display(Name = "Empresa")]
    [Required(ErrorMessage = "El campo Empresa es obligatorio.")]
    public string Empresa { get; set; } = null!;

    [Display(Name = "Cargo Anterior")]
    [Required(ErrorMessage = "El campo Cargo Anterior es obligatorio.")]
    public string Cargo { get; set; } = null!;

    [Display(Name = "Fecha Desde")]
    [Required(ErrorMessage = "El campo Fecha Desde es obligatorio.")]
    public DateOnly FechaDesde { get; set; }

    [Display(Name = "Fecha Hasta")]
    [Required(ErrorMessage = "El campo Fecha Hasta es obligatorio.")]
    public DateOnly FechaHasta { get; set; }

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [DisplayName("Activo")]
    public bool Activo { get; set; }

    [Display(Name = "Fecha Creación")]
    public DateTime FechaCreacion { get; set; }

    [Display(Name = "Fecha Modificación")]
    public DateTime FechaModificacion { get; set; }

    [Display(Name = "Creado Por")]
    public string CreadoPor { get; set; } = null!;

    [Display(Name = "Modificado Por")]
    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
