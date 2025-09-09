using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static PlanillaPM.Models.Empleado;

namespace PlanillaPM.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }


    [DisplayName("Codigo Reloj")]
    public int? CodigoReloj { get; set; }
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

    //[DisplayName("Fotografía")]
    //public byte[]? Fotografia { get; set; } = null!;

    [NotMapped]
    [DisplayName("Fotografía")]
    public byte[] Fotografia { get; set; } = null!;


    public string? FotografiaName { get; set; }

    public string? FotografiaPath { get; set; }

    [NotMapped]
    //public string? FotografiaBase64 { get; set; } = null!;

    [DisplayName("Dirección")]
    public string? Direccion { get; set; }

    [DisplayName("Teléfono")]
    [Required(ErrorMessage = "El Teléfono es obligatorio.")]
    public string Telefono { get; set; } = null!;

    [DisplayName("Ciudad de Residencia")]
    [Required(ErrorMessage = "La Ciudad de Residenciao es obligatorio.")]
    public string CiudadResidencia { get; set; } = null!;

    [DisplayName("Email")]
    public string? Email { get; set; }

    [DisplayName("Activo")]
    public bool Activo { get; set; }

    [DisplayName("Cargo")]
    [Required(ErrorMessage = "El Cargo es obligatorio.")]
    public int IdCargo { get; set; }

    [DisplayName("Departamento")]
    [Required(ErrorMessage = "El Departamento es obligatorio.")]
    public int IdDepartamento { get; set; }

    [DisplayName("Tipo de Contrato")]
    [Required(ErrorMessage = "El Tipo de Contrato es obligatorio.")]
    public int IdTipoContrato { get; set; }

    [DisplayName("Tipo de Nómina")]
    [Required(ErrorMessage = "El Tipo de Nómina es obligatorio.")]
    public int? IdTipoNomina { get; set; }

    [DisplayName("Ubicación")]
    public int? IdUbicacion { get; set; }

    [DisplayName("Encargado")]
    public int? IdEncargado { get; set; } = null!;

    [DisplayName("Fecha de Inicio")]
    public DateOnly? FechaInicio { get; set; }

    [DisplayName("Banco")]
    public int? IdBanco { get; set; } = null!;

    [DisplayName("Tipo de Cuenta")]
    public TipoCuenta? TipoCuentaBancaria { get; set; }

    [DisplayName("No. de Cuenta")]
    public string? CuentaBancaria { get; set; }

    [DisplayName("Número de RTN")]
    public string? NumeroRegistroTributario { get; set; }

    [DisplayName("Salario Base")]
    [Required(ErrorMessage = "El Salario Base es obligatorio.")]
    public decimal SalarioBase { get; set; }

    [DisplayName("No. Seguro Social")]
    public string? NumeroSeguroSocial { get; set; }

    [DisplayName("Fecha de Creación")]   
    //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]  
    //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public string? Comentarios { get; set; }

    public string? Observaciones { get; set; }

    [DisplayName("Estado Civil")]
    [Required(ErrorMessage = "El Estado Civil es obligatorio.")]
    public EstadoCivilEmpleado? EstadoCivil { get; set; }

    [DisplayName("Fecha de Inactivación")]
    public DateOnly? FechaInactivacion { get; set; }

    [DisplayName("Motivo de Inactivación")]
    public string? MotivoInactivacion { get; set; }
    [DisplayName("Clase de Empleado")]
    [Required(ErrorMessage = "La Clase de Empleado es obligatorio.")]
    public int? IdClaseEmpleado { get; set; }

    [DisplayName("Nombre Completo")]
    public string NombreCompleto
    {
        get { return string.Format("{0} {1}", NombreEmpleado, ApellidoEmpleado); }
    }

    public virtual ICollection<EmpleadoImpuesto> EmpleadoImpuestos { get; set; } = new List<EmpleadoImpuesto>();

    public virtual ICollection<VacacionDetalle> VacacionDetalles { get; set; } = new List<VacacionDetalle>();
    public virtual ICollection<Vacacion> Vacacions { get; set; } = new List<Vacacion>();
    public virtual ICollection<NominaDetalle> NominaDetalles { get; set; } = new List<NominaDetalle>();
    public virtual ICollection<HorasExtraDetalle> HorasExtraDetalles { get; set; } = new List<HorasExtraDetalle>();
    public virtual ICollection<CuentaPorCobrar> CuentaPorCobrars { get; set; } = new List<CuentaPorCobrar>();
    public virtual ICollection<EmpleadoActivo> EmpleadoActivos { get; set; } = new List<EmpleadoActivo>();

    public virtual ICollection<EmpleadoAusencium> EmpleadoAusencia { get; set; } = new List<EmpleadoAusencium>();

    public virtual ICollection<EmpleadoCargoHistorico> EmpleadoCargoHistoricos { get; set; } = new List<EmpleadoCargoHistorico>();

    public virtual ICollection<EmpleadoContacto> EmpleadoContactos { get; set; } = new List<EmpleadoContacto>();

    public virtual ICollection<EmpleadoContrato> EmpleadoContratos { get; set; } = new List<EmpleadoContrato>();

    public virtual ICollection<EmpleadoDeduccion> EmpleadoDeduccions { get; set; } = new List<EmpleadoDeduccion>();

    public virtual ICollection<EmpleadoEducacion> EmpleadoEducacions { get; set; } = new List<EmpleadoEducacion>();

    public virtual ICollection<EmpleadoExperiencium> EmpleadoExperiencia { get; set; } = new List<EmpleadoExperiencium>();

    public virtual ICollection<EmpleadoHabilidad> EmpleadoHabilidads { get; set; } = new List<EmpleadoHabilidad>();

    public virtual ICollection<EmpleadoHorario> EmpleadoHorarios { get; set; } = new List<EmpleadoHorario>();

    public virtual ICollection<EmpleadoIngreso> EmpleadoIngresos { get; set; } = new List<EmpleadoIngreso>();
    public virtual ICollection<EmpleadoHorasTrabajo> EmpleadoHorasTrabajos { get; set; } = new List<EmpleadoHorasTrabajo>();

    public virtual ICollection<EmpleadoSalarioHistorico> EmpleadoSalarioHistoricos { get; set; } = new List<EmpleadoSalarioHistorico>();



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

    public virtual ClaseEmpleado? IdClaseEmpleadoNavigation { get; set; }

    [DisplayName("Ubicación")]
    public virtual Ubicacion? IdUbicacionNavigation { get; set; } = null!;

    public virtual ICollection<Empleado> InverseIdEncargadoNavigation { get; set; } = new List<Empleado>();
    public virtual ICollection<Viatico> Viaticos { get; set; } = new List<Viatico>();

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

    [DisplayName("Antiguedad")]
    public int Antiguedad
    {
        get
        {
            DateOnly fechaActual = DateOnly.FromDateTime(DateTime.Today);
            int antiguedad = 0;

            if (FechaInicio == null)
            {
                antiguedad = 0;
            }
            else
            {
                antiguedad = fechaActual.Year - FechaInicio.Value.Year;
            }

            if (FechaInicio.HasValue && FechaInicio.Value.AddYears(antiguedad) > fechaActual)
            {
                antiguedad--;
            }

            return antiguedad;
        }
    }

    public enum EstadoCivilEmpleado
    {
        Soltero = 1,
        Casado = 2,
        Divorciado = 3,
        Viudo = 4,
        UnionLibre = 5
    }
    public enum TipoCuenta
    {
        Ahorro = 1,
        Cheque = 2
    }

    public static List<Empleado> empleados = new List<Empleado>();

    public static Dictionary<string, int> ObtenerTotalEmpleadosPorUbicacion()
    {
        // Agrupar los empleados por IdUbicacion y contar la cantidad de empleados en cada ubicación
        var empleadosPorUbicacion = empleados
            .Where(e => e.IdUbicacionNavigation != null)
            .GroupBy(e => e.IdUbicacionNavigation.NombreUbicacion)
            .Select(group => new
            {
                Ubicacion = group.Key,
                TotalEmpleados = group.Count()
            })
            .ToDictionary(x => x.Ubicacion, x => x.TotalEmpleados);

        return empleadosPorUbicacion;
    }
}


