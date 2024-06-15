using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Departamento
{
    public int IdDepartamento { get; set; }
    [DisplayName("Nombre del Departamento")]
    [Required(ErrorMessage = "El Nombre Departamento es obligatorio.")]
    public string NombreDepartamento { get; set; } = null!;

    [DisplayName("División Administrativa")]
    [Required(ErrorMessage = "El tipo de División es obligatorio.")]
    public int? IdDivision { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    [DisplayName("Division")]
    
    public virtual Division? IdDivisionNavigation { get; set; }
}