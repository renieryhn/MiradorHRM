using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanillaPM.Models;

public partial class TipoNomina
{
    public int IdTipoNomina { get; set; }
    [Display(Name = "Tipo de Planilla")]
    [Required(ErrorMessage = "El Tipo de Planilla es obligatorio.")]
    public string NombreTipoNomina { get; set; } = null!;
    [Display(Name = "Se paga cada (No. Días)")]
    [Required(ErrorMessage = "El es obligatorio.")]
    public int PagadaCadaNdias { get; set; }
    [Display(Name = "Activo")]
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

    public virtual ICollection<Nomina> Nominas { get; set; } = new List<Nomina>();
}
