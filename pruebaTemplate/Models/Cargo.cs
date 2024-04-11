using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Cargo
{
    public int IdCargo { get; set; }

    [DisplayName("Nombre del Cargo ")]
    [Required(ErrorMessage = "El Nombre del Cargo es obligatorio.")]
    public string NombreCargo { get; set; } = null!;

    [DisplayName("Funciones del Cargo ")]
    public string FuncionesCargo { get; set; } = null!;

    [DisplayName("Descripción del Cargo ")]
    public string DescripcionCargo { get; set; } = null!;

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
