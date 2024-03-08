using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoContacto
{
    public int IdContactoEmergencia { get; set; }

    public int IdEmpleado { get; set; }

    [Required(ErrorMessage = "El campo Nombre de Contacto es obligatorio.")]
    public string NombreContacto { get; set; } = null!;

    [Required(ErrorMessage = "El campo Relación es obligatorio.")]
    public string Relacion { get; set; } = null!;

    [Required(ErrorMessage = "El campo Celular es obligatorio.")]
    public string Celular { get; set; } = null!;

    public string? TelefonoFijo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
