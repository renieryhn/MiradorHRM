using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoExperiencium
{
    public int IdEmpleadoExperiencia { get; set; }

    public int IdEmpleado { get; set; }

    [Display(Name = "Empresa")]
    public string Empresa { get; set; } = null!;

    [Display(Name = "Cargo")]
    public string Cargo { get; set; } = null!;

    [Display(Name = "Fecha Desde")]
    public DateOnly FechaDesde { get; set; }

    [Display(Name = "Fecha Hasta")]
    public DateOnly FechaHasta { get; set; }

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "Fecha Creación")]
    public DateOnly FechaCreacion { get; set; }

    [Display(Name = "Fecha Modificación")]
    public DateOnly FechaModificacion { get; set; }

    [Display(Name = "Creado Por")]
    public string CreadoPor { get; set; } = null!;

    [Display(Name = "Modificado Por")]
    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Id Empleado Navigation")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
