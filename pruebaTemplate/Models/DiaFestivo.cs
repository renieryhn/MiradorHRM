using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class DiaFestivo
{
    public int IdDiaFestivo { get; set; }
    [DisplayName("Día Festivo")]
    [Required(ErrorMessage = "El Día Festivo es obligatorio.")]
    public string NombreDiaFestivo { get; set; } = null!;

    [DisplayName("Fecha Desde")]
    [Required(ErrorMessage = "La Fecha Desde es obligatorio.")]
    public DateOnly FechaDesde { get; set; }

    [DisplayName("Fecha Hasta")]
    [Required(ErrorMessage = "La Fecha Hasta es obligatorio.")]
    public DateOnly FechaHasta { get; set; }

    [DisplayName("Color")]
    [Required(ErrorMessage = "El Color es obligatorio.")]
    public string Color { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
}
