using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanillaPM.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }
    [DisplayName("Código Interno")]
    public string? CodigoInterno { get; set; }

    [DisplayName("Nombre del Empleado")]
    [Required(ErrorMessage = "El nombre del empleado es obligatorio.")]
    [StringLength(75, ErrorMessage = "El nombre del empleado no puede tener más de 75 caracteres.")]
    public string NombreEmpleado { get; set; } = null!;

    [DisplayName("Apellido del Empleado")]
    [Required(ErrorMessage = "El apellido del empleado es obligatorio.")]
    [StringLength(75, ErrorMessage = "El apellido del empleado no puede tener más de 75 caracteres.")]
    public string ApellidoEmpleado { get; set; } = null!;

    [DisplayName("Número de Identidad")]
    public string? NumeroIdentidad { get; set; }

    [DisplayName("Número de Licencia")]
    public string? NumeroLicencia { get; set; }

    [DisplayName("Fecha de Vencimiento de la Licencia")]
    public DateOnly? FechaVencimientoLicencia { get; set; }

    [DisplayName("Nacionalidad")]
    [Required(ErrorMessage = "La Nacionalidad del empleado es obligatorio.")]
    public string Nacionalidad { get; set; } = null!;

    [DisplayName("Fecha de Nacimiento")]
    [Required(ErrorMessage = "La Fecha de Nacimiento es obligatorio.")]
    public DateOnly FechaNacimiento { get; set; }

    [DisplayName("Género")]
    [Required(ErrorMessage = "El género es obligatorio.")]
    public string? Genero { get; set; }

    [DisplayName("Fotografía")]
    public byte[]? Fotografia { get; set; } = null!;

    [NotMapped]
    [DisplayName("Fotografía")]
    public IFormFile? FotoTmp { get; set; }
    [NotMapped]
    public string? FotografiaBase64 { get; set; } = null!;

    [DisplayName("Dirección")]
    public string? Direccion { get; set; }

    [DisplayName("Teléfono")]
    [Required(ErrorMessage = "El Teléfono es obligatorio.")]
    public string Telefono { get; set; } = null!;

    [DisplayName("Ciudad de Residencia")]
    public string CiudadResidencia { get; set; } = null!;

    [DisplayName("Email")]
    public string? Email { get; set; }

    [DisplayName("Activo")]
    public bool Activo { get; set; }

    [DisplayName("Cargo")]
    public int IdCargo { get; set; }

    [DisplayName("Departamento")]
    [Required(ErrorMessage = "El Departamento es obligatorio.")]
    public int IdDepartamento { get; set; }

    [DisplayName("Tipo de Contrato")]
    public int IdTipoContrato { get; set; }

    [DisplayName("Tipo de Nómina")]
    public int IdTipoNomina { get; set; }

    [DisplayName("Encargado")]
    public int? IdEncargado { get; set; } = null!;

    [DisplayName("Fecha de Inicio")]
    public DateOnly? FechaInicio { get; set; }

    [DisplayName("Banco")]
    public int? IdBanco { get; set; } = null!;

    [DisplayName("No. de Cuenta")]
    public string? CuentaBancaria { get; set; }

    [DisplayName("Número de RTN")]
    public string? NumeroRegistroTributario { get; set; }

    [DisplayName("Salario Base")]
    [Required(ErrorMessage = "El Salario Base es obligatorio.")]
    public decimal SalarioBase { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    [DisplayName("Nombre Completo")]
    public string NombreCompleto
    {
        get { return string.Format("{0} {1}", NombreEmpleado, ApellidoEmpleado); }
    }
    [DisplayName("Banco")]
    public virtual Banco? IdBancoNavigation { get; set; } = null!;
    [DisplayName("Cargo")]
    public virtual Cargo? IdCargoNavigation { get; set; } = null!;
    [DisplayName("Departamento")]
    public virtual Departamento? IdDepartamentoNavigation { get; set; } = null!;
    [DisplayName("Encargado")]
    public virtual Empleado? IdEncargadoNavigation { get; set; } = null!;
    [DisplayName("Tipo de Contrato")]
    public virtual TipoContrato? IdTipoContratoNavigation { get; set; } = null!;
    [DisplayName("Tipo de Nómina")]
    public virtual TipoNomina? IdTipoNominaNavigation { get; set; } = null!;

    public virtual ICollection<Empleado> InverseIdEncargadoNavigation { get; set; } = new List<Empleado>();

    [DisplayName("Edad")]
    public int Edad
    {
        get
        {
            // Calcular la edad utilizando la fecha de nacimiento
            DateOnly fechaActual = DateOnly.FromDateTime(DateTime.Today);
            int edad = fechaActual.Year - FechaNacimiento.Year;

            // Restar un año si el cumpleaños aún no ha pasado en el año actual
            if (FechaNacimiento.AddYears(edad) > fechaActual)
            {
                edad--;
            }

            return edad;
        }
    }

}
