using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class ClaseEmpleado
{
    public int IdClaseEmpleado { get; set; }
    [DisplayName("Nombre de Clase")]
    [Required(ErrorMessage = "El Nombre de Clase es obligatorio.")]
    public string NombreClaseEmpleado { get; set; } = null!;
    [DisplayName("Horario")]
    [Required(ErrorMessage = "El Horario es obligatorio.")]
    public int? IdHorario { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
    [DisplayName("Empleados")]
    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    [DisplayName("Horario")]
    public virtual Horario? IdHorarioNavigation { get; set; }
}

