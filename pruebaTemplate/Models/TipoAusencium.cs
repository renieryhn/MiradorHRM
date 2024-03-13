using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanillaPM.Models;

public partial class TipoAusencium
{
    public int IdTipoAusencia { get; set; }
    [Display(Name = "Tipo de Ausencia")]
    public string NombreTipoAusencia { get; set; } = null!;
    [Display(Name = "Es con Gose de Sueldo")]
    public bool GoseSueldo { get; set; }
    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<EmpleadoAusencium> EmpleadoAusencia { get; set; } = new List<EmpleadoAusencium>();
}
