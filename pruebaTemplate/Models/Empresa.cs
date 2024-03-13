using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models;


public partial class Empresa
{
    public int IdEmpresa { get; set; }
    [Display(Name = "Nombre de la Empresa")]
    public string NombreEmpresa { get; set; } = null!;
    [Display(Name = "Móneda por Defecto")]
    public int IdMoneda { get; set; }
    [Display(Name = "Lgotipo")]
    public byte[]? Logo { get; set; }
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = null!;
    [Display(Name = "Email")]
    public string? Email { get; set; }
    [Display(Name = "RTN")]
    public string? Rtn { get; set; }
    [Display(Name = "Comentarios")]
    public string? Comentarios { get; set; }

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
    [Display(Name = "Nombre de Contacto")]
    public string? NombreContacto { get; set; }
    [Display(Name = "Teléfono de Contacto")]
    public string? TelefonoContacto { get; set; }

    public virtual Monedum IdMonedaNavigation { get; set; } = null!;
}