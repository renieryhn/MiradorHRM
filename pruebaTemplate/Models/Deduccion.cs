using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Deduccion
{
    [Key]
    public int IdDeduccion { get; set; }

    [Display(Name = "Nombre de Deducción")]
    [Required(ErrorMessage = "El nombre de la deducción es requerido")]
    public string NombreDeduccion { get; set; } = null!;

    /// <summary>
    /// Porcentaje del Salario Bruto, Porcentaje del Salario Neto, Monto Fijo
    /// </summary>
    [Display(Name = "Método de Cálculo")]
    [Required(ErrorMessage = "El método de cálculo es requerido")]
    public MetodoCalculoEstado MetodoCalculo { get; set; }

    /// <summary>
    /// Seguridad Social, Aportaciones, Ahorros, Préstamos, Fondo de Pensiones, Seguro Médico, Cuotas Sindical, Fondo de Vivienda, Retención de Pensión Alimenticia, Embargo, Multas, Planes de Jubilación, Seguro de Vida, Otro
    /// </summary>
    [Display(Name = "Tipo de Deducción")]
    [Required(ErrorMessage = "El tipo de deducción es requerido")]
    public TipoDeduccionEstado TipoDeduccion { get; set; }

    /// <summary>
    /// Fijo, Fórmula o Porcentaje
    /// </summary>
    [Display(Name = "Tipo de Cálculo")]
    [Required(ErrorMessage = "El tipo de cálculo es requerido")]
    public TipoCalculoEstado TipoCalculo { get; set; } 

    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }

    [Display(Name = "Fórmula")]
    public string? Formula { get; set; }

    [Display(Name = "Orden")]
    [Required(ErrorMessage = "El orden es requerido")]
    public int Orden { get; set; }

    [Display(Name = "Deducible de Impuesto")]
    [Required(ErrorMessage = "El campo deducible de impuesto es requerido")]
    public bool DeducibleImpuesto { get; set; }

    [Display(Name = "Basado en Todo")]
    [Required(ErrorMessage = "El campo basado en todo es requerido")]
    public bool BasadoEnTodo { get; set; }

    [Display(Name = "Asignación Automática")]
    [Required(ErrorMessage = "El campo asignación automática es requerido")]
    public bool AsignacionAutomatica { get; set; }

    [Display(Name = "Activo")]
    [Required(ErrorMessage = "El campo activo es requerido")]
    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<CuentaPorCobrar> CuentaPorCobrars { get; set; } = new List<CuentaPorCobrar>();

    public virtual ICollection<DeduccionIngreso> DeduccionIngresos { get; set; } = new List<DeduccionIngreso>();

    public virtual ICollection<EmpleadoDeduccion> EmpleadoDeduccions { get; set; } = new List<EmpleadoDeduccion>();

    public virtual ICollection<NominaDeduccion> NominaDeduccions { get; set; } = new List<NominaDeduccion>();

    public enum TipoCalculoEstado
    {
        Fijo = 1,
        Fórmula = 2,
        Porcentaje = 3        
    }

    public enum MetodoCalculoEstado
    {
        [Display(Name = "Porcentaje del Salario Bruto")]
        PorcentajeDelSalarioBruto = 1,

        [Display(Name = "Porcentaje del Salario Neto")]
        PorcentajeDelSalarioNeto = 2,

        [Display(Name = "Monto Fijo")]
        MontoFijo = 3
    }

    public enum TipoDeduccionEstado
    {
        [Display(Name = "Seguridad Social")]
        SeguridadSocial = 1,

        [Display(Name = "Aportaciones")]
        Aportaciones = 2,

        [Display(Name = "Ahorros")]
        Ahorros = 3,

        [Display(Name = "Préstamos")]
        Prestamos = 4,

        [Display(Name = "Fondo de Pensiones")]
        FondoDePensiones = 5,

        [Display(Name = "Seguro Médico")]
        SeguroMedico = 6,

        [Display(Name = "Cuotas Sindical")]
        CuotasSindical = 7,

        [Display(Name = "Fondo de Vivienda")]
        FondoDeVivienda = 8,

        [Display(Name = "Retención de Pensión Alimenticia")]
        RetencionDePensionAlimenticia = 9,

        [Display(Name = "Embargo")]
        Embargo = 10,

        [Display(Name = "Multas")]
        Multas = 11,

        [Display(Name = "Planes de Jubilación")]
        PlanesDeJubilacion = 12,

        [Display(Name = "Seguro de Vida")]
        SeguroDeVida = 13,

        [Display(Name = "Otro")]
        Otro = 14
    }




}
