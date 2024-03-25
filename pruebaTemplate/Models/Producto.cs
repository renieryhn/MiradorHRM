using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PlanillaPM.Models;

public partial class Producto
{
    public int IdProducto { get; set; }
    [Display(Name = "Nombre del Producto o Activo Fijo")]
    [Required(ErrorMessage = "El Nombre del Producto o Activo Fijo es obligatorio.")]
    public string NombreProducto { get; set; } = null!;
    [Display(Name = "Código")]
    public string? CodigoProducto { get; set; }
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<EmpleadoActivo> EmpleadoActivos { get; set; } = new List<EmpleadoActivo>();
}
