using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Ingreso
{
    public string IdIngreso { get; set; } = null!;

    [Display(Name = "Nombre de Ingreso")]
    public string NombreIngreso { get; set; } = null!;

    /// <summary>
    /// Fijo, Fórmula o Porcentaje
    /// </summary>
    /// Salario, Por Hora, Jornal, Comisión, Horas Extra, Paga Doble, Vacaciones, Enfermedad, Días Libre, Bono, Incentivo, Otro
    [Display(Name = "Tipo de Ingreso")]
    public string Tipo { get; set; } = null!;
    [Display(Name = "Monto")]
    public decimal? Monto { get; set; }
    //Semanal, Quincenal, Mensual, Trimestral, Semestral, Anual, Diario
    [Display(Name = "Período")]
    public int Periodo { get; set; }
    //Basdo en pago: y factor de pago porcentade de basado en .
    [Display(Name = "Fórmula")]
    public string? Formula { get; set; }
    [Display(Name = "Es Grabable")]
    public bool Grabable { get; set; }
    [Display(Name = "Ordenación")]
    public int Orden { get; set; }
    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    [DisplayName("Fecha Inicial")]
    public DateTime FechaInicial { get; set; }

    [DisplayName("Fecha Final")]
    public DateTime FechaFinal { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<EmpleadoIngreso> EmpleadoIngresos { get; set; } = new List<EmpleadoIngreso>();

    public enum TipoIngreso
    {
        Fijo,
        Fórmula,
        Porcentaje
    }
}
