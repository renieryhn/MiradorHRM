using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;

public partial class Horario
{
    public int IdHorario { get; set; }
    [Display(Name = "Horario")]
    public string NombreHorario { get; set; } = null!;
    [Display(Name = "Tipo de Horario")]
    public int IdTipoHorario { get; set; }
    [Display(Name = "Turno No.")]
    public string? TurnoNumero { get; set; }

    [Display(Name = "Trabaja Lunes")]
    public bool IndLunes { get; set; }
    [Display(Name = "Trabaja Martes")]
    public bool IndMartes { get; set; }
    [Display(Name = "Trabaja Miércoles")]
    public bool IndMiercoles { get; set; }
    [Display(Name = "Trabaja Jueves")]
    public bool IndJueves { get; set; }
    [Display(Name = "Trabaja Viernes")]
    public bool IndViernes { get; set; }
    [Display(Name = "Trabaja Sábado")]
    public bool IndSabado { get; set; }
    [Display(Name = "Trabaja Domingo")]
    public bool IndDomingo { get; set; }
    [Display(Name = "Lun. Desde")]
    public TimeOnly? LunDesde { get; set; }
    [Display(Name = "Lun. Hasta")]
    public TimeOnly? LunHasta { get; set; }
    [Display(Name = "Mar. Desde")]
    public TimeOnly? MarDesde { get; set; }
    [Display(Name = "Mar. Hasta")]
    public TimeOnly? MarHasta { get; set; }
    [Display(Name = "Mie. Desde")]
    public TimeOnly? MieDesde { get; set; }
    [Display(Name = "Mie. Hasta")]
    public TimeOnly? MieHasta { get; set; }
    [Display(Name = "Jue. Desde")]
    public TimeOnly? JueDesde { get; set; }
    [Display(Name = "Jue. Hasta")]
    public TimeOnly? JueHasta { get; set; }
    [Display(Name = "Vie. Desde")]
    public TimeOnly? VieDesde { get; set; }
    [Display(Name = "Vie. Hasta")]
    public TimeOnly? VieHasta { get; set; }
    [Display(Name = "Sab. Desde")]
    public TimeOnly? SabDesde { get; set; }
    [Display(Name = "Sab. Hasta")]
    public TimeOnly? SabHasta { get; set; }
    [Display(Name = "Dom. Desde")]
    public TimeOnly? DomDesde { get; set; }
    [Display(Name = "Dom. Hasta")]
    public TimeOnly? DomHasta { get; set; }
    [Display(Name = "Receso a Hora de Comida")]
    public bool? IndComida { get; set; }
    [Display(Name = "Comidad Desde")]
    public TimeOnly? ComidaDesde { get; set; }
    [Display(Name = "Comifa Hasta")]
    public TimeOnly? ComidaHasta { get; set; }
    [Display(Name = "Total de Horas por Semana")]
    public string? TotalHorasSemana { get; set; }

    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<ClaseEmpleado> ClaseEmpleados { get; set; } = new List<ClaseEmpleado>();

    public virtual ICollection<EmpleadoHorario> EmpleadoHorarios { get; set; } = new List<EmpleadoHorario>();

    public virtual TipoHorario IdTipoHorarioNavigation { get; set; } = null!;
}
