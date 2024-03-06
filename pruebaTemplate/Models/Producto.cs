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

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<EmpleadoActivo> EmpleadoActivos { get; set; } = new List<EmpleadoActivo>();
}
