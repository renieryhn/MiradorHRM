using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoAusencium
{
    public int IdEmpleadoAusencia { get; set; }

    public int IdEmpleado { get; set; }

    public int IdTipoAusencia { get; set; }

    public bool DiaCompleto { get; set; }

    /// <summary>
    /// Solicitada/Aprobada/Rechazada
    /// </summary>
    public int Estado { get; set; }

    public DateOnly FechaDesde { get; set; }

    public DateOnly FechaHasta { get; set; }

    public TimeOnly? HoraDesde { get; set; }

    public TimeOnly? HoraHasta { get; set; }

    public string? AprobadoPor { get; set; }

    public string? Comentarios { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual TipoAusencium IdTipoAusenciaNavigation { get; set; } = null!;
}
