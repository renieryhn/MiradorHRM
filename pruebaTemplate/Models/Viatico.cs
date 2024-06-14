using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Viatico
{
    [Display(Name = "ID del Viático")]
    public int IdViatico { get; set; }

    [Display(Name = "ID del Empleado")]
    [Required(ErrorMessage = "El ID del empleado es requerido")]
    public int IdEmpleado { get; set; }

    [Display(Name = "Descripción")]
    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
    public string Descripcion { get; set; } = null!;

    [Display(Name = "Fecha")]
    [Required(ErrorMessage = "La fecha es requerida")]
    public DateOnly Fecha { get; set; }

    [Display(Name = "Total de Gastos")]
    [Required(ErrorMessage = "El total de gastos es requerido")]
    [Range(0, 9999999999999999.99, ErrorMessage = "El total de gastos debe estar entre 0 y 9999999999999999.99")]
    public decimal TotalGastos { get; set; }

    [Display(Name = "Adelanto Recibido")]
    [Required(ErrorMessage = "El adelanto recibido es requerido")]
    [Range(0, 9999999999999999.99, ErrorMessage = "El adelanto recibido debe estar entre 0 y 9999999999999999.99")]
    public decimal AdelantoRecibido { get; set; }

    [Display(Name = "Balance Pendiente")]
    [Required(ErrorMessage = "El balance pendiente es requerido")]
    [Range(0, 9999999999999999.99, ErrorMessage = "El balance pendiente debe estar entre 0 y 9999999999999999.99")]
    public decimal BalancePendiente { get; set; }

    /// <summary>
    /// Pendiente, Aprobado, Rechazado
    /// </summary>
    [Display(Name = "Estado")]
    [Required(ErrorMessage = "El estado es requerido")]
    public TipoEstado Estado { get; set; }

    [Display(Name = "Fecha de Aprobación")]
    public DateOnly? FechaAprobacion { get; set; }

    [Display(Name = "Aprobado Por")]
    [StringLength(50, ErrorMessage = "El nombre del aprobador no puede exceder los 50 caracteres")]
    public string? AprobadoPor { get; set; }

    [Display(Name = "Notas Adicionales")]
    public string? NotasAdicionales { get; set; }

    [Display(Name = "Pagado")]
    [Required(ErrorMessage = "El campo Pagado es requerido")]
    public bool Pagado { get; set; }

    [Display(Name = "Fecha de Pago")]
    public DateOnly? FechaPago { get; set; }

    [Display(Name = "Activo")]
    [Required(ErrorMessage = "El campo Activo es requerido")]
    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual ICollection<ViaticoDetalle> ViaticoDetalles { get; set; } = new List<ViaticoDetalle>();

    public enum TipoEstado
    {       
        Pendiente = 1, 
        Aprobado = 2,
        Rechazado = 3
    }
}
