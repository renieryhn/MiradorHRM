using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class EmpleadoActivo
{
    public int IdEmpleadoActivo { get; set; }
    [DisplayName("Empleado")]
    public int IdEmpleado { get; set; }
    [DisplayName("Producto/Activo Fijo")]
    public int IdProducto { get; set; }
    [DisplayName("Modelo del Producto")]
    public string? Model { get; set; }
    [DisplayName("Número de Serie del Producto")]
    public string? NumeroSerie { get; set; }

    /// <summary>
    /// Nuevo/Usado/Reacondicionado
    /// </summary>
    [DisplayName("Estado Actual")]
    public int Estado { get; set; }
    [DisplayName("Cantidad")]
    public int Cantidad { get; set; }
    [DisplayName("Precio Estimado")]
    public decimal PrecioEstimado { get; set; }
    [DisplayName("Fecha de Asignación")]
    public DateOnly FechaAsignacion { get; set; }
    [DisplayName("Descripción")]
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
    [DisplayName("Empleados")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
    [DisplayName("Productos")]
    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public enum EstadoActual
    {
        Nuevo,
        Usado,
        Reacondicionado
    }
}
