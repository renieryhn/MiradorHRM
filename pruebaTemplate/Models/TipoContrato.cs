using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class TipoContrato
{
    public int IdTipoContrato { get; set; }

    public string NombreTipoContrato { get; set; } = null!;

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
