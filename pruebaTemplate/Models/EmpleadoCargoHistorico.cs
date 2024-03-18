using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoCargoHistorico
{
    public int IdEmpleadoCargo { get; set; }
    [Display(Name = "Empleado")]
    public int IdEmpleado { get; set; }
    [Display(Name = "Cargo Desempeñado")]
    public int IdCargo { get; set; }
    [Display(Name = "Comentarios")]
    public string? Comentario { get; set; }
    [DisplayName("Activo")]
    public bool Activo { get; set; }
    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
