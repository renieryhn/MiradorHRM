using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class TipoHorario
{
    public int IdTipoHorario { get; set; }
    [Display(Name = "Tipo de Horario")]
    public string NombreTipoHorario { get; set; } = null!;
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

    public virtual ICollection<Horario> Horarios { get; set; } = new List<Horario>();
}
