using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlanillaPM.Models;

public partial class ArchivoAdjunto
{

    public int IdArchivo { get; set; }

    [DisplayName("Id Relación")]
    public int IdRelatedObject { get; set; }

    [DisplayName("Relacionado a")]
    public string ObjectName { get; set; } = null!;

    [DisplayName("Activo")]
    public bool Activo { get; set; }

    [DisplayName("Adjunto")]
    public byte[] Archivo { get; set; } = null!;

    [DisplayName("Tamaño")]
    public decimal ArchivoSize { get; set; }

    [DisplayName("Nombre de Archivo")]
    public string ArchivoNombre { get; set; } = null!;

    [DisplayName("Ruta de Archivo")]
    public string? ArchivoPath { get; set; }

    [DisplayName("Tipo de Archivo")]
    public string ArchivoTipo { get; set; } = null!;

    [DisplayName("Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    [DisplayName("Fehca de Modificación")]
    public DateTime FechaModificacion { get; set; }

    [DisplayName("Creado Por")]
    public string? CreadoPor { get; set; }

    [DisplayName("Modificado Por")]
    public string? ModificadoPor { get; set; }
}
