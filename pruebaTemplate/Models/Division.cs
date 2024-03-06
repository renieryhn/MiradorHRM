using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class Division
{
    public int IdDivision { get; set; }

    public int NombreDivision { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
}
