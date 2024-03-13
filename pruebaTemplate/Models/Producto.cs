using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string NombreProducto { get; set; } = null!;

    public string? CodigoProducto { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<EmpleadoActivo> EmpleadoActivos { get; set; } = new List<EmpleadoActivo>();
}
