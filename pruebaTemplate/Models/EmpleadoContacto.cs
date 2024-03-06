using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoContacto
{
    public int IdContactoEmergencia { get; set; }

    public int IdEmpleado { get; set; }

    public string NombreContacto { get; set; } = null!;

    /// <summary>
    /// Cónyugue, Hermano, Primo, Amigo, Etc.
    /// </summary>
    public string Relacion { get; set; } = null!;

    public string Celular { get; set; } = null!;

    public string? TelefonoFijo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
