using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class TipoContrato
{
    public int IdTipoContrato { get; set; }

    public string NombreTipoContrato { get; set; } = null!;

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
