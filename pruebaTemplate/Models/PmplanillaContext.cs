using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PlanillaPM.Models;

public partial class PmplanillaContext : DbContext
{
    public PmplanillaContext()
    {
    }

    public PmplanillaContext(DbContextOptions<PmplanillaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Empleado> Empleados { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=26.129.51.59;Database=PMPLANILLA;User Id=sa;Password=@dmin2024;Trusted_Connection=False;Encrypt=False;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

            entity.HasOne(d => d.IdEncargadoNavigation).WithMany(p => p.InverseIdEncargadoNavigation)
                .HasForeignKey(d => d.IdEncargado)
                .HasConstraintName("FK_Empleado_EmpleadoEncargado");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
