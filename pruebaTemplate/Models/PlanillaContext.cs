using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using pruebaTemplate.Models;

namespace PlanillaPM.Models;

public partial class PlanillaContext : IdentityDbContext<Usuario>
{
    public PlanillaContext(DbContextOptions<PlanillaContext> options)
        : base(options)
    {
    }
    [Display(Name = "Bancos Nacionales")]
    public virtual DbSet<Banco> Bancos { get; set; }
    [Display(Name = "Cargo de Empleados")]
    public virtual DbSet<Cargo> Cargos { get; set; }
    [Display(Name = "Clase e Empleado")]
    public virtual DbSet<ClaseEmpleado> ClaseEmpleados { get; set; }

    public virtual DbSet<Deduccion> Deduccions { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<DiaFestivo> DiaFestivos { get; set; }

    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EmpleadoActivo> EmpleadoActivos { get; set; }

    public virtual DbSet<EmpleadoAusencium> EmpleadoAusencia { get; set; }

    public virtual DbSet<EmpleadoCargoHistorico> EmpleadoCargoHistoricos { get; set; }

    public virtual DbSet<EmpleadoContacto> EmpleadoContactos { get; set; }

    public virtual DbSet<EmpleadoContrato> EmpleadoContratos { get; set; }

    public virtual DbSet<EmpleadoDeduccion> EmpleadoDeduccions { get; set; }

    public virtual DbSet<EmpleadoEducacion> EmpleadoEducacions { get; set; }

    public virtual DbSet<EmpleadoExperiencium> EmpleadoExperiencia { get; set; }

    public virtual DbSet<EmpleadoHabilidad> EmpleadoHabilidads { get; set; }

    public virtual DbSet<EmpleadoHorario> EmpleadoHorarios { get; set; }

    public virtual DbSet<EmpleadoIngreso> EmpleadoIngresos { get; set; }

    public virtual DbSet<EmpleadoSalarioHistorico> EmpleadoSalarioHistoricos { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Horario> Horarios { get; set; }

    public virtual DbSet<Impuesto> Impuestos { get; set; }

    public virtual DbSet<ImpuestoTabla> ImpuestoTablas { get; set; }

    public virtual DbSet<Ingreso> Ingresos { get; set; }

    public virtual DbSet<Monedum> Moneda { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<TipoAusencium> TipoAusencia { get; set; }

    public virtual DbSet<TipoContrato> TipoContratos { get; set; }

    public virtual DbSet<TipoHorario> TipoHorarios { get; set; }

    public virtual DbSet<TipoNomina> TipoNominas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:sDBConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
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
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreCargo)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<ClaseEmpleado>(entity =>
        {
            entity.HasKey(e => e.IdClaseEmpleado);

            entity.ToTable("ClaseEmpleado");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreClaseEmpleado)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.IdHorarioNavigation).WithMany(p => p.ClaseEmpleados)
                .HasForeignKey(d => d.IdHorario)
                .HasConstraintName("FK_ClaseEmpleado_Horario");
        });

        modelBuilder.Entity<Deduccion>(entity =>
        {
            entity.HasKey(e => e.IdDeduccion);

            entity.ToTable("Deduccion");

            entity.Property(e => e.IdDeduccion).HasMaxLength(20);
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.BasadoEnTodo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Formula).HasMaxLength(4000);
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Monto).HasColumnType("numeric(18, 4)");
            entity.Property(e => e.NombreDeduccion).HasMaxLength(50);
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasDefaultValue("Fijo")
                .HasComment("Fijo, Fórmula o Porcentaje");
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

            entity.HasOne(d => d.IdDivisionNavigation).WithMany(p => p.Departamentos)
                .HasForeignKey(d => d.IdDivision)
                .HasConstraintName("FK_Departamento_Division");
        });

        modelBuilder.Entity<DiaFestivo>(entity =>
        {
            entity.HasKey(e => e.IdDiaFestivo);

            entity.ToTable("DiaFestivo");

            entity.Property(e => e.IdDiaFestivo).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreDiaFestivo).HasMaxLength(50);
        });

        modelBuilder.Entity<Division>(entity =>
        {
            entity.HasKey(e => e.IdDivision);

            entity.ToTable("Division");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
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
            entity.Property(e => e.MotivoInactivacion).HasMaxLength(1000);
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

            entity.HasOne(d => d.IdClaseEmpleadoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdClaseEmpleado)
                .HasConstraintName("FK_Empleado_ClaseEmpleado");

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

        modelBuilder.Entity<EmpleadoActivo>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoActivo);

            entity.ToTable("EmpleadoActivo");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.Estado)
                .HasDefaultValue(1)
                .HasComment("Nuevo/Usado/Reacondicionado");
            entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NumeroSerie).HasMaxLength(50);
            entity.Property(e => e.PrecioEstimado).HasColumnType("numeric(18, 2)");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoActivos)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoActivo_Empleado");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.EmpleadoActivos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoActivo_Producto");
        });

        modelBuilder.Entity<EmpleadoAusencium>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoAusencia);

            entity.Property(e => e.AprobadoPor).HasMaxLength(50);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.DiaCompleto).HasDefaultValue(true);
            entity.Property(e => e.Estado).HasComment("Solicitada/Aprobada/Rechazada");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoAusencia)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoAusencia_Empleado");

            entity.HasOne(d => d.IdTipoAusenciaNavigation).WithMany(p => p.EmpleadoAusencia)
                .HasForeignKey(d => d.IdTipoAusencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoAusencia_TipoAusencia");
        });

        modelBuilder.Entity<EmpleadoCargoHistorico>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoCargo);

            entity.ToTable("EmpleadoCargoHistorico");

            entity.Property(e => e.IdEmpleadoCargo).ValueGeneratedNever();
            entity.Property(e => e.Comentario).HasMaxLength(1000);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoCargoHistoricos)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoCargoHistorico_Empleado");
        });

        modelBuilder.Entity<EmpleadoContacto>(entity =>
        {
            entity.HasKey(e => e.IdContactoEmergencia);

            entity.ToTable("EmpleadoContacto");

            entity.Property(e => e.Celular).HasMaxLength(10);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreContacto).HasMaxLength(100);
            entity.Property(e => e.Relacion)
                .HasMaxLength(50)
                .HasComment("Cónyugue, Hermano, Primo, Amigo, Etc.");
            entity.Property(e => e.TelefonoFijo).HasMaxLength(10);

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoContactos)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoContacto_Empleado");
        });

        modelBuilder.Entity<EmpleadoContrato>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoContrato);

            entity.ToTable("EmpleadoContrato");

            entity.Property(e => e.CodigoContrato).HasMaxLength(20);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Salario).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.VigenciaMeses).HasDefaultValue(6);

            entity.HasOne(d => d.IdCargoNavigation).WithMany(p => p.EmpleadoContratos)
                .HasForeignKey(d => d.IdCargo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoContrato_Cargo");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoContratos)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoContrato_Empleado");
        });

        modelBuilder.Entity<EmpleadoDeduccion>(entity =>
        {
            entity.HasKey(e => new { e.IdDeduccion, e.IdEmpleado });

            entity.ToTable("EmpleadoDeduccion");

            entity.Property(e => e.IdDeduccion).HasMaxLength(20);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.IdDeduccionNavigation).WithMany(p => p.EmpleadoDeduccions)
                .HasForeignKey(d => d.IdDeduccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoDeduccion_Deduccion");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoDeduccions)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoDeduccion_Empleado");
        });

        modelBuilder.Entity<EmpleadoEducacion>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoEducacion);

            entity.ToTable("EmpleadoEducacion");

            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Institucion).HasMaxLength(50);
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.TituloObtenido).HasMaxLength(70);

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoEducacions)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoEducacion_Empleado");
        });

        modelBuilder.Entity<EmpleadoExperiencium>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoExperiencia);

            entity.Property(e => e.IdEmpleadoExperiencia).ValueGeneratedNever();
            entity.Property(e => e.Cargo).HasMaxLength(70);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Empresa).HasMaxLength(50);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoExperiencia)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoExperiencia_Empleado");
        });

        modelBuilder.Entity<EmpleadoHabilidad>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoHabilidad);

            entity.ToTable("EmpleadoHabilidad");

            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Habilidad).HasMaxLength(100);
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoHabilidads)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoHabilidad_Empleado");
        });

        modelBuilder.Entity<EmpleadoHorario>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoHorario).HasName("PK_EmpleadoTurno");

            entity.ToTable("EmpleadoHorario");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IndComida).HasDefaultValue(false);
            entity.Property(e => e.IndJueves).HasDefaultValue(true);
            entity.Property(e => e.IndLunes).HasDefaultValue(true);
            entity.Property(e => e.IndMartes).HasDefaultValue(true);
            entity.Property(e => e.IndMiercoles).HasDefaultValue(true);
            entity.Property(e => e.IndViernes).HasDefaultValue(true);
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.TotalHorasSemana).HasMaxLength(5);

            entity.HasOne(d => d.IdHorarioBaseNavigation).WithMany(p => p.EmpleadoHorarios)
                .HasForeignKey(d => d.IdHorarioBase)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoHorario_Horario");

            entity.HasOne(d => d.IdempleadoNavigation).WithMany(p => p.EmpleadoHorarios)
                .HasForeignKey(d => d.Idempleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoHorario_Empleado");
        });

        modelBuilder.Entity<EmpleadoIngreso>(entity =>
        {
            entity.HasKey(e => new { e.IdIngreso, e.IdEmpleado });

            entity.ToTable("EmpleadoIngreso");

            entity.Property(e => e.IdIngreso).HasMaxLength(20);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoIngresos)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoIngreso_Empleado");

            entity.HasOne(d => d.IdIngresoNavigation).WithMany(p => p.EmpleadoIngresos)
                .HasForeignKey(d => d.IdIngreso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoIngreso_Ingreso");
        });

        modelBuilder.Entity<EmpleadoSalarioHistorico>(entity =>
        {
            entity.HasKey(e => e.IdEmpleadoSalarioHistorico);

            entity.ToTable("EmpleadoSalarioHistorico");

            entity.Property(e => e.Comentario).HasMaxLength(1000);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.SalarioActual).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.SalarioAnterior).HasColumnType("numeric(18, 2)");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoSalarioHistoricos)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmpleadoSalarioHistorico_Empleado");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa);

            entity.ToTable("Empresa");

            entity.Property(e => e.Comentarios).HasMaxLength(4000);
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.ModificadoPor).HasMaxLength(50);
            entity.Property(e => e.NombreContacto).HasMaxLength(100);
            entity.Property(e => e.NombreEmpresa).HasMaxLength(100);
            entity.Property(e => e.Rtn)
                .HasMaxLength(20)
                .HasColumnName("RTN");
            entity.Property(e => e.Telefono).HasMaxLength(10);
            entity.Property(e => e.TelefonoContacto).HasMaxLength(10);

            entity.HasOne(d => d.IdMonedaNavigation).WithMany(p => p.Empresas)
                .HasForeignKey(d => d.IdMoneda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empresa_Moneda");
        });

        modelBuilder.Entity<Horario>(entity =>
        {
            entity.HasKey(e => e.IdHorario);

            entity.ToTable("Horario");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IndComida).HasDefaultValue(false);
            entity.Property(e => e.IndJueves).HasDefaultValue(true);
            entity.Property(e => e.IndLunes).HasDefaultValue(true);
            entity.Property(e => e.IndMartes).HasDefaultValue(true);
            entity.Property(e => e.IndMiercoles).HasDefaultValue(true);
            entity.Property(e => e.IndViernes).HasDefaultValue(true);
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreHorario).HasMaxLength(50);
            entity.Property(e => e.TotalHorasSemana).HasMaxLength(5);
            entity.Property(e => e.TurnoNumero)
                .HasMaxLength(10)
                .HasDefaultValue("DIURNO");

            entity.HasOne(d => d.IdTipoHorarioNavigation).WithMany(p => p.Horarios)
                .HasForeignKey(d => d.IdTipoHorario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Horario_TipoHorario");
        });

        modelBuilder.Entity<Impuesto>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Impuesto");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Formula).HasMaxLength(4000);
            entity.Property(e => e.IdImpuesto).HasMaxLength(20);
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Monto).HasColumnType("numeric(18, 4)");
            entity.Property(e => e.NombreImpuesto).HasMaxLength(50);
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasDefaultValue("Fijo")
                .HasComment("Fijo, Fórmula, Porcentaje o Tabla");
        });

        modelBuilder.Entity<ImpuestoTabla>(entity =>
        {
            entity.HasKey(e => e.IdImpuestoTabla);

            entity.ToTable("ImpuestoTabla");

            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Desde).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Hasta).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.IdImpuesto).HasMaxLength(20);
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Monto).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Porcentaje).HasColumnType("numeric(3, 2)");
        });

        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.HasKey(e => e.IdIngreso);

            entity.ToTable("Ingreso");

            entity.Property(e => e.IdIngreso).HasMaxLength(20);
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Formula).HasMaxLength(4000);
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Monto).HasColumnType("numeric(18, 4)");
            entity.Property(e => e.NombreIngreso).HasMaxLength(50);
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasDefaultValue("Fijo")
                .HasComment("Fijo, Fórmula o Porcentaje");
        });

        modelBuilder.Entity<Monedum>(entity =>
        {
            entity.HasKey(e => e.IdMoneda);

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreMoneda).HasMaxLength(50);
            entity.Property(e => e.Simbolo).HasMaxLength(4);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto);

            entity.ToTable("Producto");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CodigoProducto).HasMaxLength(20);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreProducto).HasMaxLength(100);
        });

        modelBuilder.Entity<TipoAusencium>(entity =>
        {
            entity.HasKey(e => e.IdTipoAusencia);

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreTipoAusencia).HasMaxLength(100);
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

        modelBuilder.Entity<TipoHorario>(entity =>
        {
            entity.HasKey(e => e.IdTipoHorario);

            entity.ToTable("TipoHorario");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaModificacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModificadoPor)
                .HasMaxLength(50)
                .HasDefaultValue("Admin")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NombreTipoHorario)
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
