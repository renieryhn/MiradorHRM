using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class CuentaPorCobrar
{
    [Key]
    public int IdCuentaPorCobrar { get; set; }

    [Display(Name = "ID Empleado")]
    [Required(ErrorMessage = "El ID del empleado es requerido")]
    public int IdEmpleado { get; set; }

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "ID Deducción")]
    [Required(ErrorMessage = "El ID de la deducción es requerido")]
    public int IdDeduccion { get; set; }

    [Display(Name = "Fecha de Inicio")]
    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateOnly FechaInicio { get; set; }

    [Display(Name = "Monto")]
    [Required(ErrorMessage = "El monto es requerido")]
    public decimal Monto { get; set; }

    [Display(Name = "Interés Aplicado")]
    [Required(ErrorMessage = "El interés aplicado es requerido")]
    public decimal InteresAplicado { get; set; }

    [Display(Name = "Número de Pagos")]
    [Required(ErrorMessage = "El número de pagos es requerido")]
    public int NumeroPagos { get; set; }

    [Display(Name = "Fecha de Finalización")]
    [Required(ErrorMessage = "La fecha de finalización es requerida")]
    public DateOnly FechaFinalizacion { get; set; }

    [Display(Name = "Estado de Cuenta por Cobrar")]
    [Required(ErrorMessage = "El estado de cuenta por cobrar es requerido")]
    public CuentaPorCobrarEst EstadoCuentaPorCobrar { get; set; }

    [Display(Name = "Estado de Aprobación")]
    [Required(ErrorMessage = "El estado de aprobación es requerido")]
    public Aprobacion EstadoAprobacion { get; set; }

    [Display(Name = "Aprobado por")]
    public string? AprobadoPor { get; set; } = null!;

    [Display(Name = "Comentario de Aprobación")]
    public string? ComentarioAprobacion { get; set; } = null!;

    [Display(Name = "Activo")]
    [Required(ErrorMessage = "El campo Activo es requerido")]
    public bool Activo { get; set; }

    [Display(Name = "Fecha de Creación")]
    public DateTime? FechaCreacion { get; set; } = null!;

    [Display(Name = "Fecha de Modificación")]
    public DateTime? FechaModificacion { get; set; } = null!;

    [Display(Name = "Creado por")]

    public string? CreadoPor { get; set; } = null!;

    [Display(Name = "Modificado por")]

    public string? ModificadoPor { get; set; } = null!;

    public virtual ICollection<CuentaPorCobrarDetalle> CuentaPorCobrarDetalles { get; set; } = new List<CuentaPorCobrarDetalle>();

    [Display(Name = "Deducción")]
    public virtual Deduccion IdDeduccionNavigation { get; set; } 

    [Display(Name = "Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } 

    public enum Aprobacion
    {
        Solicitado = 1,
        Aprobado = 2,
        Rechazado = 3
    }  
    public enum CuentaPorCobrarEst
    {
        [Display(Name = "Activo")]
        Activo = 1,

        [Display(Name = "Pagado")]
        Pagado = 2,

        [Display(Name = "En Mora")]
        EnMora = 3    
    }
}
