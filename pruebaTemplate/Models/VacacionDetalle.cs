using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class VacacionDetalle
{
    public int IdVacacionDetalle { get; set; }

    public int IdVacacion { get; set; }

    public int IdEmpleado { get; set; }

    public DateOnly FechaSolicitud { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int NumeroDiasSolicitados { get; set; }

    /// <summary>
    /// Pendiente, Aprobada, Rechazada
    /// </summary>
    public int EstadoSolicitud { get; set; }

    public string? AprobadoPor { get; set; }

    public int DiasAprobados { get; set; }

    public string? ComentariosAprobador { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Vacacion IdVacacionNavigation { get; set; } = null!;
}
