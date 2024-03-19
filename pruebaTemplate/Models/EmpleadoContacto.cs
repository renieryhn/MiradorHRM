using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class EmpleadoContacto
{
    public int IdContactoEmergencia { get; set; }

    [Display(Name = "Id Empleado")]
    public int IdEmpleado { get; set; }

    [Required(ErrorMessage = "El campo Nombre de Contacto es obligatorio.")]
    [Display(Name = "Nombre de Contacto")]
    public string NombreContacto { get; set; } = null!;

    [Required(ErrorMessage = "El campo Relación es obligatorio.")]
    [Display(Name = "Relación")]
    public string Relacion { get; set; } = null!;

    [Required(ErrorMessage = "El campo Celular es obligatorio.")]
    [Display(Name = "Celular")]
    public string Celular { get; set; } = null!;

    [Display(Name = "Teléfono Fijo")]
    public string? TelefonoFijo { get; set; }

    [DisplayName("Activo")]
    public bool Activo { get; set; }

    [Display(Name = "Fecha Creación")]
    public DateTime FechaCreacion { get; set; }

    [Display(Name = "Fecha Modificación")]
    public DateTime FechaModificacion { get; set; }

    [Display(Name = "Creado Por")]
    public string CreadoPor { get; set; } = null!;

    [Display(Name = "Modificado Por")]
    public string ModificadoPor { get; set; } = null!;

    [Display(Name = "Id Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
