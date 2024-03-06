using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class TipoAusencium
{
    public int IdTipoAusencia { get; set; }

    public string NombreTipoAusencia { get; set; } = null!;

    public bool GoseSueldo { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<EmpleadoAusencium> EmpleadoAusencia { get; set; } = new List<EmpleadoAusencium>();
}
