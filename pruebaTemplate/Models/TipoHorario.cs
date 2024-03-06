using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class TipoHorario
{
    public int IdTipoHorario { get; set; }

    public string NombreTipoHorario { get; set; } = null!;

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<Horario> Horarios { get; set; } = new List<Horario>();
}
