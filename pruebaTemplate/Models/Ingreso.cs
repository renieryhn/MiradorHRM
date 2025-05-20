using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Ingreso
{
    [Display(Name = "ID del Ingreso")]
    public int IdIngreso { get; set; }

    [Display(Name = "Nombre del Ingreso")]
    [Required(ErrorMessage = "El nombre del ingreso es requerido")]
    [StringLength(50, ErrorMessage = "El nombre del ingreso no puede exceder los 50 caracteres")]
    public string NombreIngreso { get; set; } = null!;

    [Display(Name = "Tipo de Ingreso")]
    [Required(ErrorMessage = "El tipo de ingreso es requerido")]
    public TipoIngresoEstado TipoIngreso { get; set; }

    [Display(Name = "Tipo de Cálculo")]
    [Required(ErrorMessage = "El tipo de cálculo es requerido")]
    public TipoCalculoEstado TipoCalculo { get; set; }

    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }

    [Display(Name = "Fórmula")]
    
    [StringLength(4000, ErrorMessage = "La fórmula no puede exceder los 4000 caracteres")]
    public string? Formula { get; set; }

    [Display(Name = "Grabable")]
    [Required(ErrorMessage = "El campo Grabable es requerido")]
    public bool Grabable { get; set; }

    [Display(Name = "Asignación Automática")]
    [Required(ErrorMessage = "El campo Asignación Automática es requerido")]
    public bool AsignacionAutomatica { get; set; }

    [Display(Name = "Orden")]
    [Required(ErrorMessage = "El orden es requerido")]
    public int Orden { get; set; }

    [Display(Name = "Activo")]
    [Required(ErrorMessage = "El campo Activo es requerido")]
    public bool Activo { get; set; }

    [Display(Name = "Fecha de Creación")]

    public DateTime FechaCreacion { get; set; }

    [Display(Name = "Fecha de Modificación")]

    public DateTime FechaModificacion { get; set; }

    [Display(Name = "Creado Por")]

    public string CreadoPor { get; set; } = null!;

    [Display(Name = "Modificado Por")]
 
    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Fecha Inicial")]
    public DateOnly? FechaInicial { get; set; }

    [Display(Name = "Fecha Final")]
    public DateOnly? FechaFinal { get; set; }

    [Display(Name = "Periodo de Pago")]
    [Required(ErrorMessage = "El período es requerido")]
    public int Periodo { get; set; }

    public virtual ICollection<DeduccionIngreso> DeduccionIngresos { get; set; } = new List<DeduccionIngreso>();

    public virtual ICollection<EmpleadoIngreso> EmpleadoIngresos { get; set; } = new List<EmpleadoIngreso>();

    public virtual ICollection<NominaIngreso> NominaIngresos { get; set; } = new List<NominaIngreso>();


    public enum TipoCalculoEstado
    {
        Fijo = 1,
        Fórmula = 2,
        Porcentaje = 3
    }
    
    public enum TipoIngresoEstado
    {
        [Display(Name = "Salario")]
        Salario = 1,

        [Display(Name = "Bono de Productividad")]
        BonoDeProductividad = 2,

        [Display(Name = "Comisión")]
        Comision = 3,

        [Display(Name = "Horas Extra")]
        HorasExtra = 4,

        [Display(Name = "Vacaciones")]
        Vacaciones = 5,

        [Display(Name = "Aguinaldo")]
        Aguinaldo = 6,

        [Display(Name = "Días Libres")]
        DiasLibres = 7,

        [Display(Name = "Pensión")]
        Pension = 8,

        [Display(Name = "Propina")]
        Propina = 9,

        [Display(Name = "Por Meta")]
        PorMeta = 10,

        [Display(Name = "Indemnización")]
        Indemnizacion = 11,

        [Display(Name = "Reembolso de Gastos")]
        ReembolsoDeGastos = 12,

        [Display(Name = "Viáticos")]
        Viaticos = 13,

        [Display(Name = "Bono Navideño")]
        BonoNavideno = 14,

        [Display(Name = "Otros")]
        Otros = 15
    }

}
