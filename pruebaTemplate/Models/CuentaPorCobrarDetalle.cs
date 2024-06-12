using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class CuentaPorCobrarDetalle
{
    public int IdCuentaPorCobrarDetalle { get; set; }

    public int IdCuentaPorCobrar { get; set; }

    public int NumeroPago { get; set; }

    public DateOnly Fecha { get; set; }

    public decimal Cuota { get; set; }

    public decimal MontoPendiente { get; set; }

    public decimal MontoPagado { get; set; }

    public decimal Interes { get; set; }

    /// <summary>
    /// Pendiente, Pagada
    /// </summary>
    public int EstadoCuota { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual CuentaPorCobrar IdCuentaPorCobrarNavigation { get; set; } = null!;
}
