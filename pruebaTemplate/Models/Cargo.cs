using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class Cargo
{
    public int IdCargo { get; set; }
    [DisplayName("Nombre del Cargo ")]
    public string NombreCargo { get; set; } = null!;

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<EmpleadoContrato> EmpleadoContratos { get; set; } = new List<EmpleadoContrato>();

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
