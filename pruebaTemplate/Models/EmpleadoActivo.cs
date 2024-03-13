using System;
using System.Collections.Generic;
using System.ComponentModel;

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

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
