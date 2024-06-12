using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class HorasExtra
{
    public int IdHorasExtra { get; set; }

    public int Periodo { get; set; }

    public int Mes { get; set; }

    public int TotalEmpleados { get; set; }

    public string? AprobadoPor { get; set; }

    public string? Comentarios { get; set; }

    public DateOnly? FechaApobacion { get; set; }

    public int? IdNomina { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<HorasExtraDetalle> HorasExtraDetalles { get; set; } = new List<HorasExtraDetalle>();

    public virtual Nomina? IdNominaNavigation { get; set; }
}
