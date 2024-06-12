using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class HorasExtraDetalle
{
    public int IdHorasExtraDetalle { get; set; }

    public int IdHorasExtra { get; set; }

    public int IdEmpleado { get; set; }

    public decimal TotalNormales { get; set; }

    public decimal TotalDiurna { get; set; }

    public decimal TotalNocturna { get; set; }

    public decimal TotalMixta { get; set; }

    public decimal TotalNoTrabajado { get; set; }

    /// <summary>
    /// Pendiente,Aprobado, Rechazado
    /// </summary>
    public int EstadoAprobacion { get; set; }

    public string? AprobadoPor { get; set; }

    public string? CometarioAprobador { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual HorasExtra IdHorasExtraNavigation { get; set; } = null!;
}
