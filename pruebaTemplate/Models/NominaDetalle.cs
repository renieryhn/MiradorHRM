using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class NominaDetalle
{
    public int IdNominaDetalle { get; set; }

    public int IdNomina { get; set; }

    public int IdEmpleado { get; set; }

    public decimal SalarioBase { get; set; }

    public decimal Ingresos { get; set; }

    public decimal Deducciones { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Neto { get; set; }

    public string? Comentarios { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Nomina IdNominaNavigation { get; set; } = null!;

    public virtual ICollection<NominaDeduccion> NominaDeduccions { get; set; } = new List<NominaDeduccion>();

    public virtual ICollection<NominaImpuesto> NominaImpuestos { get; set; } = new List<NominaImpuesto>();

    public virtual ICollection<NominaIngreso> NominaIngresos { get; set; } = new List<NominaIngreso>();
}
