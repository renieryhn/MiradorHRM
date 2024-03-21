using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class DiaFestivo
{
    public int IdDiaFestivo { get; set; }
    [DisplayName("Día Festivo")]
    public string NombreDiaFestivo { get; set; } = null!;

    [DisplayName("Fecha Desde")]
    public DateOnly FechaDesde { get; set; }

    [DisplayName("Fecha Hasta")]
    public DateOnly FechaHasta { get; set; }

    [DisplayName("Color")]
    public string Color { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
}
