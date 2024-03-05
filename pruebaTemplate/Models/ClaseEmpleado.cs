using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class ClaseEmpleado
{
    public int IdClaseEmpleado { get; set; }

    public string NombreClaseEmpleado { get; set; } = null!;

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;
}
