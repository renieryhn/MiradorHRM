using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Monedum
{
    public int IdMoneda { get; set; }
    [Display(Name = "Nombre de la Moneda")]
    [Required(ErrorMessage = "El Nombre de la Moneda es obligatorio.")]
    public string NombreMoneda { get; set; } = null!;
    [Display(Name = "Símbolo")]
    [Required(ErrorMessage = "El Símbolo es obligatorio.")]
    public string Simbolo { get; set; } = null!;
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

    public virtual ICollection<Empresa> Empresas { get; set; } = new List<Empresa>();
}
