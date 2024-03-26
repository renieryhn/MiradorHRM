using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [Display(Name = "Tipo de Contrato")]
    public int IdTipoContrato { get; set; }
    [Display(Name = "Cargo")]
    public int IdCargo { get; set; }
    [Display(Name = "Estado")]
    public int Estado { get; set; }
    [Display(Name = "Vegencia del Contrato (Meses)")]
    public int VigenciaMeses { get; set; }
    [Display(Name = "Fecha de Inicio")]
    public DateOnly FechaInicio { get; set; }
    [Display(Name = "Fecha de Finalización")]
    public DateOnly FechaFin { get; set; }
    [Display(Name = "Salario")]
    public decimal Salario { get; set; }
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

    [Display(Name = "Cargo")]
    public virtual Cargo IdCargoNavigation { get; set; } = null!;

    [Display(Name = "Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

}

