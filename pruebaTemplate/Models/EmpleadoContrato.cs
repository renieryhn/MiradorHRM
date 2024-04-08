using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static PlanillaPM.Models.EmpleadoActivo;

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
    [Required(ErrorMessage = "El Tipo de Contrato es obligatorio.")]
    public int IdTipoContrato { get; set; }
    [Display(Name = "Cargo")]
    [Required(ErrorMessage = "El Cargo es obligatorio.")]
    public int IdCargo { get; set; }
    [Display(Name = "Estado")]
    [Required(ErrorMessage = "El Estado es obligatorio.")]
    public EstadoContrato Estado { get; set; }
    [Display(Name = "Vegencia del Contrato (Meses)")]
    [Required(ErrorMessage = "El campo Vegencia del Contrato es obligatorio.")]
    public int VigenciaMeses { get; set; }
    [Display(Name = "Fecha de Inicio")]
    [Required(ErrorMessage = "La Fecha de Inicio es obligatorio.")]
    public DateOnly FechaInicio { get; set; }
    [Display(Name = "Fecha de Finalización")]
    [Required(ErrorMessage = "La Fecha de Finalización es obligatorio.")]
    public DateOnly FechaFin { get; set; }
    [Display(Name = "Salario")]
    [Required(ErrorMessage = "El Salario es obligatorio.")]
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
    
    [Display(Name = "Tipo Contrato")]
    public virtual TipoContrato IdTipoContratoNavigation { get; set; } = null!;

    public enum EstadoContrato
    {
        Borrador = 1,
        Aprobado = 2,
        Cancelado = 3,
        Finalizado = 4
    }
}


