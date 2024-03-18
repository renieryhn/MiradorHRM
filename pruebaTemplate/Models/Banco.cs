using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace PlanillaPM.Models;

public partial class Banco
{
    public int IdBanco { get; set; }
    [DisplayName("Nombre del Banco")]
    public string NombreBanco { get; set; } = null!;

    [DisplayName("Activo")]
    public bool Activo { get; set; }

    [DisplayName("Fecha de Creación")]
     //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fecha de Modificación")]
    //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
