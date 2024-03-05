using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public string? CodigoInterno { get; set; }

    public string NombreEmpleado { get; set; } = null!;

    public string ApellidoEmpleado { get; set; } = null!;

    public string? NumeroIdentidad { get; set; }

    public string? NumeroLicencia { get; set; }

    public DateOnly? FechaVencimientoLicencia { get; set; }

    public string Nacionalidad { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string? Genero { get; set; }

    public byte[]? Fotografia { get; set; }

    public string? Direccion { get; set; }

    public string Telefono { get; set; } = null!;

    public string CiudadResidencia { get; set; } = null!;

    public string? Email { get; set; }

    public bool Activo { get; set; }

    public int IdCargo { get; set; }

    public int IdDepartamento { get; set; }

    public int IdTipoContrato { get; set; }

    public int IdTipoNomina { get; set; }

    public int? IdEncargado { get; set; }

    public int? IdClaseEmpleado { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public int? IdBanco { get; set; }

    public string? CuentaBancaria { get; set; }

    public string? NumeroRegistroTributario { get; set; }

    public decimal SalarioBase { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public string? Comentarios { get; set; }

    public string? Observaciones { get; set; }

    public virtual Empleado? IdEncargadoNavigation { get; set; }

    public virtual ICollection<Empleado> InverseIdEncargadoNavigation { get; set; } = new List<Empleado>();
}
