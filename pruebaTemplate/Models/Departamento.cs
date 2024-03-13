using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class Departamento
{
    public int IdDepartamento { get; set; }
    [DisplayName("Nombre del Departamento")]
    public string NombreDepartamento { get; set; } = null!;

    [DisplayName("División Administrativa")]
    public int? IdDivision { get; set; }

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

    public virtual Division? IdDivisionNavigation { get; set; }
}