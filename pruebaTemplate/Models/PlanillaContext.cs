using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using pruebaTemplate.Models;

namespace PlanillaPM.Models;

public partial class PlanillaContext : DbContext
{
    public PlanillaContext()
    {
    }

    public PlanillaContext(DbContextOptions options) 
        : base(options)
    {
    }

    public virtual DbSet<ArchivoAdjunto> ArchivoAdjuntos { get; set; }

    public virtual DbSet<Banco> Bancos { get; set; }

    public virtual DbSet<Cargo> Cargos { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<TipoContrato> TipoContratos { get; set; }

    public virtual DbSet<TipoNomina> TipoNominas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:sDBConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArchivoAdjunto>(entity =>
        {
            entity.HasKey(e => e.IdArchivo);

            entity.ToTable("ArchivoAdjunto");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.ArchivoNombre).HasMaxLength(1000);
            entity.Property(e => e.ArchivoPath).HasMaxLength(1000);
            entity.Property(e => e.ArchivoSize).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ArchivoTipo).HasMaxLength(1000);
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ModificadoPor).HasMaxLength(50);
            entity.Property(e => e.ObjectName)
                .HasMaxLength(25)
                .HasDefaultValue("Documento");
        });

        modelBuilder.Entity<Banco>(entity =>
        {
            entity.HasKey(e => e.IdBanco);

            entity.ToTable("Banco");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreBanco)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasKey(e => e.IdCargo);

            entity.ToTable("Cargo");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreCargo)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento);

            entity.ToTable("Departamento");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreDepartamento)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("PK__Empleado__CE6D8B9EC12710B3");

            entity.ToTable("Empleado");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.ApellidoEmpleado).HasMaxLength(75);
            entity.Property(e => e.CiudadResidencia).HasMaxLength(75);
            entity.Property(e => e.CodigoInterno).HasMaxLength(20);
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.CuentaBancaria).HasMaxLength(20);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Genero).HasMaxLength(20);
            entity.Property(e => e.ModificadoPor).HasMaxLength(50);
            entity.Property(e => e.NombreEmpleado).HasMaxLength(75);
            entity.Property(e => e.NumeroIdentidad).HasMaxLength(20);
            entity.Property(e => e.NumeroLicencia).HasMaxLength(20);
            entity.Property(e => e.NumeroRegistroTributario).HasMaxLength(20);
            entity.Property(e => e.SalarioBase).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Telefono).HasMaxLength(10);

            entity.HasOne(d => d.IdBancoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdBanco)
                .HasConstraintName("FK_Empleado_Banco");

            entity.HasOne(d => d.IdCargoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdCargo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleado_Cargo");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdDepartamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleado_Departamento");

            entity.HasOne(d => d.IdEncargadoNavigation).WithMany(p => p.InverseIdEncargadoNavigation)
                .HasForeignKey(d => d.IdEncargado)
                .HasConstraintName("FK_Empleado_EmpleadoEncargado");

            entity.HasOne(d => d.IdTipoContratoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdTipoContrato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleado_TipoContrato");

            entity.HasOne(d => d.IdTipoNominaNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdTipoNomina)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleado_TipoNomina");
        });

        modelBuilder.Entity<TipoContrato>(entity =>
        {
            entity.HasKey(e => e.IdTipoContrato);

            entity.ToTable("TipoContrato");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreTipoContrato)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<TipoNomina>(entity =>
        {
            entity.HasKey(e => e.IdTipoNomina);

            entity.ToTable("TipoNomina");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreTipoNomina)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.PagadaCadaNdias).HasColumnName("PagadaCadaNDias");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
