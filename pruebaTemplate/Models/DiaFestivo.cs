using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class DiaFestivo
{
    public int IdDiaFestivo { get; set; }

    public string NombreDiaFestivo { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;
}
