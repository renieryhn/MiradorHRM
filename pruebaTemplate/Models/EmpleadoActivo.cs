using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanillaPM.Models;

public partial class EmpleadoActivo
{
    public int IdEmpleadoActivo { get; set; }
    [DisplayName("Empleado")]
    [Required(ErrorMessage = "El Empleado es obligatorio.")]
    public int IdEmpleado { get; set; }
    [DisplayName("Producto/Activo Fijo")]
    [Required(ErrorMessage = "El Producto/Activo Fijo es obligatorio.")]
    public int IdProducto { get; set; }
    [DisplayName("Modelo del Producto")]
    [Required(ErrorMessage = "El Modelo del Producto es obligatorio.")]
    public string? Model { get; set; }
    [DisplayName("Número de Serie del Producto")]
    [Required(ErrorMessage = "El Número de Serie del Producto es obligatorio.")]
    public string? NumeroSerie { get; set; }

    /// <summary>
    /// Nuevo/Usado/Reacondicionado
    /// </summary>
    [DisplayName("Estado Actual")]
    [Required(ErrorMessage = "El Estado Actual es obligatorio.")]
    public EstadoActual Estado { get; set; }
    [DisplayName("Cantidad")]
    [Required(ErrorMessage = "La Cantidad es obligatorio.")]
    public int Cantidad { get; set; }
    [DisplayName("Precio Estimado")]
    [Required(ErrorMessage = "El Precio Estimado es obligatorio.")]
    public decimal PrecioEstimado { get; set; }
    [DisplayName("Fecha de Asignación")]
    public DateOnly FechaAsignacion { get; set; }
    [DisplayName("Descripción")]
    public string? Descripcion { get; set; }

    [DisplayName("Activo")]
    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
    [DisplayName("Empleado")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
    [DisplayName("Productos")]
    public virtual Producto IdProductoNavigation { get; set; } = null!;


    public enum EstadoActual
    {
        Nuevo=1,
        Usado=2,
        Reacondicionado=3,
            test=4
    }
}
