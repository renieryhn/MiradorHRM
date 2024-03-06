using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;


public partial class Empresa
{
    public int IdEmpresa { get; set; }

    public string NombreEmpresa { get; set; } = null!;

    public int IdMoneda { get; set; }

    public byte[]? Logo { get; set; }

    public string? Direccion { get; set; }

    public string Telefono { get; set; } = null!;

    public string? Email { get; set; }

    public string? Rtn { get; set; }

    public string? Comentarios { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;

    public string? NombreContacto { get; set; }

    public string? TelefonoContacto { get; set; }

    public virtual Monedum IdMonedaNavigation { get; set; } = null!;
}