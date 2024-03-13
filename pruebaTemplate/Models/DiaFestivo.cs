using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class DiaFestivo
{
    public int IdDiaFestivo { get; set; }
    [DisplayName("Día Festivo")]
    public string NombreDiaFestivo { get; set; } = null!;

    [DisplayName("Fecha")]
    public DateOnly Fecha { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
}
