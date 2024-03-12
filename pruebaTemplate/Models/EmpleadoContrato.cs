using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoContrato
{
    public int IdEmpleadoContrato { get; set; }

    [Display(Name = "ID Empleado")]
    public int IdEmpleado { get; set; }

    [Required(ErrorMessage = "El campo Código de Contrato es obligatorio.")]
    [Display(Name = "Código de Contrato")]
    public string CodigoContrato { get; set; } = null!;

    public int IdTipoContrato { get; set; }

    public int IdCargo { get; set; }

    public int Estado { get; set; }

    public int VigenciaMeses { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal Salario { get; set; }

    public string? Descripcion { get; set; }

    [Display(Name = "Fecha Creación")]
    public DateOnly FechaCreacion { get; set; }

    [Display(Name = "Fecha Modificación")]
    public DateOnly FechaModificacion { get; set; }

    [Display(Name = "Creado Por")]
    public string CreadoPor { get; set; } = null!;

    [Display(Name = "Modificado Por")]
    public string ModificadoPor { get; set; } = null!;

    public virtual Cargo IdCargoNavigation { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

}

