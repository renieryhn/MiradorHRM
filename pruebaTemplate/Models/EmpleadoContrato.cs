using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoContrato
{
    public int IdEmpleadoContrato { get; set; }

    public int IdEmpleado { get; set; }

    public string CodigoContrato { get; set; } = null!;

    public int IdTipoContrato { get; set; }

    public int IdCargo { get; set; }

    public int Estado { get; set; }

    public int VigenciaMeses { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal Salario { get; set; }

    public string? Descripcion { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Cargo IdCargoNavigation { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}

