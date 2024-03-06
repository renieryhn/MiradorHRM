using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class EmpleadoActivo
{
    public int IdEmpleadoActivo { get; set; }

    public int IdEmpleado { get; set; }

    public int IdProducto { get; set; }

    public string? Model { get; set; }

    public string? NumeroSerie { get; set; }

    /// <summary>
    /// Nuevo/Usado/Reacondicionado
    /// </summary>
    public int Estado { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioEstimado { get; set; }

    public DateOnly FechaAsignacion { get; set; }

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
