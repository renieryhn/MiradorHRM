using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class Horario
{
    public int IdHorario { get; set; }

    public string NombreHorario { get; set; } = null!;

    public int IdTipoHorario { get; set; }

    public string? TurnoNumero { get; set; }

    public bool IndLunes { get; set; }

    public bool IndMartes { get; set; }

    public bool IndMiercoles { get; set; }

    public bool IndJueves { get; set; }

    public bool IndViernes { get; set; }

    public bool IndSabado { get; set; }

    public bool IndDomingo { get; set; }

    public TimeOnly? LunDesde { get; set; }

    public TimeOnly? LunHasta { get; set; }

    public TimeOnly? MarDesde { get; set; }

    public TimeOnly? MarHasta { get; set; }

    public TimeOnly? MieDesde { get; set; }

    public TimeOnly? MieHasta { get; set; }

    public TimeOnly? JueDesde { get; set; }

    public TimeOnly? JueHasta { get; set; }

    public TimeOnly? VieDesde { get; set; }

    public TimeOnly? VieHasta { get; set; }

    public TimeOnly? SabDesde { get; set; }

    public TimeOnly? SabHasta { get; set; }

    public TimeOnly? DomDesde { get; set; }

    public TimeOnly? DomHasta { get; set; }

    public bool? IndComida { get; set; }

    public TimeOnly? ComidaDesde { get; set; }

    public TimeOnly? ComidaHasta { get; set; }

    public string? TotalHorasSemana { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public virtual ICollection<ClaseEmpleado> ClaseEmpleados { get; set; } = new List<ClaseEmpleado>();

    public virtual ICollection<EmpleadoHorario> EmpleadoHorarios { get; set; } = new List<EmpleadoHorario>();

    public virtual TipoHorario IdTipoHorarioNavigation { get; set; } = null!;
}
