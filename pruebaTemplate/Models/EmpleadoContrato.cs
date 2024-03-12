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

    [Required(ErrorMessage = "El campo Tipo de Contrato es obligatorio.")]
    [Display(Name = "ID Tipo de Contrato")]
    public int IdTipoContrato { get; set; }

    [Required(ErrorMessage = "El campo ID de Cargo es obligatorio.")]
    [Display(Name = "ID de Cargo")]
    public int IdCargo { get; set; }

    [Required(ErrorMessage = "El campo Estado es obligatorio.")]
    [Display(Name = "Estado")]
    public int Estado { get; set; }

    [Required(ErrorMessage = "El campo Vigencia en Meses es obligatorio.")]
    [Display(Name = "Vigencia en Meses")]
    public int VigenciaMeses { get; set; }

    [DataType(DataType.Date)] 
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Fecha de Inicio")]
    public DateTime FechaInicio { get; set; }

    [DataType(DataType.Date)]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Display(Name = "Fecha de Fin")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "El campo Salario es obligatorio.")]
    [Display(Name = "Salario")]
    public decimal Salario { get; set; }

    [Display(Name = "Descripción")]
    public string Descripcion { get; set; }

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

