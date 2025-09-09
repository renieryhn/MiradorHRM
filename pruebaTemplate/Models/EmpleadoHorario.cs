using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanillaPM.Models;

public class EmpleadoHorario
{
    public int IdEmpleadoHorario { get; set; }

    [Display(Name = "Empleado")]
    public int IdEmpleado { get; set; }

    [Display(Name = "Horario")]
    public int? IdHorarioBase { get; set; }

    [Display(Name = "Trabaja Sábado")]
    public bool IndSabado { get; set; }

    [Display(Name = "Trabaja Domingo")]
    public bool IndDomingo { get; set; }

    [Display(Name = "Lun. Desde")]
    public TimeSpan? LunDesde { get; set; }

    [Display(Name = "Lun. Hasta")]
    public TimeSpan? LunHasta { get; set; }

    [Display(Name = "Mar. Desde")]
    public TimeSpan? MarDesde { get; set; }

    [Display(Name = "Mar. Hasta")]
    public TimeSpan? MarHasta { get; set; }

    [Display(Name = "Mie. Desde")]
    public TimeSpan? MieDesde { get; set; }

    [Display(Name = "Mie. Hasta")]
    public TimeSpan? MieHasta { get; set; }

    [Display(Name = "Jue. Desde")]
    public TimeSpan? JueDesde { get; set; }

    [Display(Name = "Jue. Hasta")]
    public TimeSpan? JueHasta { get; set; }

    [Display(Name = "Vie. Desde")]
    public TimeSpan? VieDesde { get; set; }

    [Display(Name = "Vie. Hasta")]
    public TimeSpan? VieHasta { get; set; }

    [Display(Name = "Sab. Desde")]
    public TimeSpan? SabDesde { get; set; }

    [Display(Name = "Sab. Hasta")]
    public TimeSpan? SabHasta { get; set; }

    [Display(Name = "Dom. Desde")]
    public TimeSpan? DomDesde { get; set; }

    [Display(Name = "Dom. Hasta")]
    public TimeSpan? DomHasta { get; set; }
 

    [Display(Name = "Total de Horas por Semana")]
    public string? TotalHorasSemana { get; set; }

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
    public virtual Empleado? IdEmpleadoNavigation { get; set; } = null!;



    [Display(Name = "Lun. Receso Desde")]
    public TimeSpan? LunRecesoDesde { get; set; }

    [Display(Name = "Lun. Receso Hasta")]
    public TimeSpan? LunRecesoHasta { get; set; }

    [Display(Name = "Mar. Receso Desde")]
    public TimeSpan? MarRecesoDesde { get; set; }

    [Display(Name = "Mar. Receso Hasta")]
    public TimeSpan? MarRecesoHasta { get; set; }

    [Display(Name = "Mie. Receso Desde")]
    public TimeSpan? MieRecesoDesde { get; set; }

    [Display(Name = "Mie. Receso Hasta")]
    public TimeSpan? MieRecesoHasta { get; set; }

    [Display(Name = "Jue. Receso Desde")]
    public TimeSpan? JueRecesoDesde { get; set; }

    [Display(Name = "Jue. Receso Hasta")]
    public TimeSpan? JueRecesoHasta { get; set; }

    [Display(Name = "Vie. Receso Desde")]
    public TimeSpan? VieRecesoDesde { get; set; }

    [Display(Name = "Vie. Receso Hasta")]
    public TimeSpan? VieRecesoHasta { get; set; }

    [Display(Name = "Sab. Receso Desde")]
    public TimeSpan? SabRecesoDesde { get; set; }

    [Display(Name = "Sab. Receso Hasta")]
    public TimeSpan? SabRecesoHasta { get; set; }

    [Display(Name = "Dom. Receso Desde")]
    public TimeSpan? DomRecesoDesde { get; set; }

    [Display(Name = "Dom. Receso Hasta")]
    public TimeSpan? DomRecesoHasta { get; set; }

    public virtual Horario? IdHorarioBaseNavigation { get; set; } = null!;

    public string? HorasLunes { get; set; }

    public string? HorasMartes { get; set; }

    public string? HorasMiercoles { get; set; }

    public string? HorasJueves { get; set; }

    public string? HorasViernes { get; set; }

    public string? HorasSabado { get; set; }

    public string? HorasDomingo { get; set; }

}
