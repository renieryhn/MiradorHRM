using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Nomina
{
    [Key]
    public int IdNomina { get; set; }

    [Display(Name = "Tipo de Nómina")]
    [Required(ErrorMessage = "El Id de Tipo de Nómina es requerido")]
    public int IdTipoNomina { get; set; }

    [Display(Name = "Comentarios")]
    [Required(ErrorMessage = "Los comentarios son requeridos")]
    public string Comentarios { get; set; } = null!;

    [Display(Name = "Periodo Fiscal")]
    [Required(ErrorMessage = "El periodo fiscal es requerido")]
    public int PeriodoFiscal { get; set; }

    [Display(Name = "Mes")]
    [Required(ErrorMessage = "El mes es requerido")]
    public int Mes { get; set; }

    [Display(Name = "Fecha de Pago")]
    [Required(ErrorMessage = "La fecha de pago es requerida")]
    public DateOnly FechaPago { get; set; }

    [Display(Name = "Total de Ingresos")]
    [Required(ErrorMessage = "El total de ingresos es requerido")]
    public decimal TotalIngresos { get; set; }

    [Display(Name = "Total de Deducciones")]
    [Required(ErrorMessage = "El total de deducciones es requerido")]
    public decimal TotalDeducciones { get; set; }

    [Display(Name = "Total de Impuestos")]
    [Required(ErrorMessage = "El total de impuestos es requerido")]
    public decimal TotalImpuestos { get; set; }

    [Display(Name = "Total de Empleados en Nómina")]
    [Required(ErrorMessage = "El total de empleados en nómina es requerido")]
    public int TotalEmpleadosEnNomina { get; set; }

    [Display(Name = "Pago Neto")]
    [Required(ErrorMessage = "El pago neto es requerido")]
    public decimal PagoNeto { get; set; }

    /// <summary>
    /// En Trabajo, Pendiente de Aprobación, Aprobada, Rechazada, Pagada o Finalizada
    /// </summary>
    [Display(Name = "Estado de Nómina")]
    [Required(ErrorMessage = "El estado de la nómina es requerido")]
    public NominaEstado EstadoNomina { get; set; }

    [Display(Name = "Aprobada Por")]
    public string? AprobadaPor { get; set; }

    [Display(Name = "Comentarios del Aprobador")]
    public string? ComentariosAprobador { get; set; }

    [Display(Name = "Activo")]
    [Required(ErrorMessage = "El campo Activo es requerido")]
    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<HorasExtra> HorasExtras { get; set; } = new List<HorasExtra>();

    [Display(Name = "Tipo Nomina")]
    [Required(ErrorMessage = "El campo Tipo Nomina es requerido")]
    public virtual TipoNomina IdTipoNominaNavigation { get; set; } = null!;

    public virtual ICollection<NominaDetalle> NominaDetalles { get; set; } = new List<NominaDetalle>();

    public enum NominaEstado
    {
        EnTrabajo = 1,
        PendientedeAprobación = 2,
        Aprobada = 3,
        Rechazada = 4,
        Pagada = 5,
        Finalizada = 6
    }
}
