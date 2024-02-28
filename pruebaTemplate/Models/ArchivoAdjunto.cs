using System;
using System.Collections.Generic;

namespace PlanillaPM.Models;

public partial class ArchivoAdjunto
{
    public int IdArchivo { get; set; }

    public int IdRelatedObject { get; set; }

    public string ObjectName { get; set; } = null!;

    public bool Activo { get; set; }

    public byte[] Archivo { get; set; } = null!;

    public decimal ArchivoSize { get; set; }

    public string ArchivoNombre { get; set; } = null!;

    public string? ArchivoPath { get; set; }

    public string ArchivoTipo { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public DateOnly FechaModificacion { get; set; }

    public string CreadoPor { get; set; } = null!;

    public string ModificadoPor { get; set; } = null!;
}
