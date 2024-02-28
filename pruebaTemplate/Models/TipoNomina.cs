using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class TipoNomina
{
    public int IdTipoNomina { get; set; }

    public string NombreTipoNomina { get; set; } = null!;

    public int PagadaCadaNdias { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
