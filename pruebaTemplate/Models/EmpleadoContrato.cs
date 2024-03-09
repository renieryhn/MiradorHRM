using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoContrato
{
    public int IdEmpleadoContrato { get; set; }

    public int IdEmpleado { get; set; }

    [Required(ErrorMessage = "El campo Código de Contrato es obligatorio.")]
    public string CodigoContrato { get; set; } = null!;

    [Required(ErrorMessage = "El campo Tipo de Contrato es obligatorio.")]
    public int IdTipoContrato { get; set; }

    [Required(ErrorMessage = "El campo ID de Cargo es obligatorio.")]
    public int IdCargo { get; set; }

    [Required(ErrorMessage = "El campo Estado es obligatorio.")]
    public int Estado { get; set; }

    [Required(ErrorMessage = "El campo Vigencia en Meses es obligatorio.")]
    public int VigenciaMeses { get; set; }

    [DataType(DataType.Date)] 
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public DateTime FechaInicio { get; set; }

    [DataType(DataType.Date)]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "El campo Salario es obligatorio.")]
    public decimal Salario { get; set; }


    public string Descripcion { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Cargo IdCargoNavigation { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}

