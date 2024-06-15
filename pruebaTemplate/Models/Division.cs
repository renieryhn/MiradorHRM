using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Division
{

    public List<Division> Divisions { get; set; }
    public bool CanEdit { get; set; }
    public bool CanViewDetails { get; set; }
    public bool CanDelete { get; set; }

    public int IdDivision { get; set; }
    [DisplayName("Nombre de la División Administrativa")]
    [Required(ErrorMessage = "El Nombre Division Administrativa es obligatorio.")]
    public string NombreDivision { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
}
