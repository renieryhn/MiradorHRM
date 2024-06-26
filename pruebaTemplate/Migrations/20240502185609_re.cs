using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanillaPM.Migrations
{
    /// <inheritdoc />
    public partial class re : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "ArchivoAdjunto",
                columns: table => new
                {
                    IdArchivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRelatedObject = table.Column<int>(type: "int", nullable: false),
                    ObjectName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true, defaultValue: "Documento"),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Archivo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ArchivoSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ArchivoNombre = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ArchivoPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ArchivoTipo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivoAdjunto", x => x.IdArchivo);
                });

            migrationBuilder.CreateTable(
                name: "Banco",
                columns: table => new
                {
                    IdBanco = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreBanco = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banco", x => x.IdBanco);
                });

            migrationBuilder.CreateTable(
                name: "Cargo",
                columns: table => new
                {
                    IdCargo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCargo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    FuncionesCargo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescripcionCargo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargo", x => x.IdCargo);
                });

            migrationBuilder.CreateTable(
                name: "Deduccion",
                columns: table => new
                {
                    IdDeduccion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreDeduccion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Fijo", comment: "Fijo, Fórmula o Porcentaje"),
                    Monto = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Formula = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DeducibleImpuesto = table.Column<bool>(type: "bit", nullable: false),
                    BasadoEnTodo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deduccion", x => x.IdDeduccion);
                });

            migrationBuilder.CreateTable(
                name: "DiaFestivo",
                columns: table => new
                {
                    IdDiaFestivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreDiaFestivo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaDesde = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaHasta = table.Column<DateOnly>(type: "date", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaFestivo", x => x.IdDiaFestivo);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    IdDivision = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreDivision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.IdDivision);
                });

            migrationBuilder.CreateTable(
                name: "Impuesto",
                columns: table => new
                {
                    IdImpuesto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NombreImpuesto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Fijo", comment: "Fijo, Fórmula, Porcentaje o Tabla"),
                    Monto = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Formula = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Grabable = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ImpuestoTabla",
                columns: table => new
                {
                    IdImpuestoTabla = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdImpuesto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Desde = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Hasta = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Porcentaje = table.Column<decimal>(type: "numeric(3,2)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpuestoTabla", x => x.IdImpuestoTabla);
                });

            migrationBuilder.CreateTable(
                name: "Ingreso",
                columns: table => new
                {
                    IdIngreso = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreIngreso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Fijo", comment: "Fijo, Fórmula o Porcentaje"),
                    Monto = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Periodo = table.Column<int>(type: "int", nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Grabable = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaInicial = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingreso", x => x.IdIngreso);
                });

            migrationBuilder.CreateTable(
                name: "Moneda",
                columns: table => new
                {
                    IdMoneda = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreMoneda = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Simbolo = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moneda", x => x.IdMoneda);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreProducto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoProducto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.IdProducto);
                });

            migrationBuilder.CreateTable(
                name: "TipoAusencia",
                columns: table => new
                {
                    IdTipoAusencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTipoAusencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GoseSueldo = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoAusencia", x => x.IdTipoAusencia);
                });

            migrationBuilder.CreateTable(
                name: "TipoContrato",
                columns: table => new
                {
                    IdTipoContrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTipoContrato = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoContrato", x => x.IdTipoContrato);
                });

            migrationBuilder.CreateTable(
                name: "TipoHorario",
                columns: table => new
                {
                    IdTipoHorario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTipoHorario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoHorario", x => x.IdTipoHorario);
                });

            migrationBuilder.CreateTable(
                name: "TipoNomina",
                columns: table => new
                {
                    IdTipoNomina = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreTipoNomina = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    PagadaCadaNDias = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoNomina", x => x.IdTipoNomina);
                });

            migrationBuilder.CreateTable(
                name: "Departamento",
                columns: table => new
                {
                    IdDepartamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreDepartamento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    IdDivision = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamento", x => x.IdDepartamento);
                    table.ForeignKey(
                        name: "FK_Departamento_Division",
                        column: x => x.IdDivision,
                        principalTable: "Division",
                        principalColumn: "IdDivision");
                });

            migrationBuilder.CreateTable(
                name: "Empresa",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEmpresa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdMoneda = table.Column<int>(type: "int", nullable: false),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RTN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Comentarios = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreContacto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TelefonoContacto = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.IdEmpresa);
                    table.ForeignKey(
                        name: "FK_Empresa_Moneda",
                        column: x => x.IdMoneda,
                        principalTable: "Moneda",
                        principalColumn: "IdMoneda");
                });

            migrationBuilder.CreateTable(
                name: "Horario",
                columns: table => new
                {
                    IdHorario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreHorario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdTipoHorario = table.Column<int>(type: "int", nullable: false),
                    TurnoNumero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, defaultValue: "DIURNO"),
                    IndLunes = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndMartes = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndMiercoles = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndJueves = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndViernes = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndSabado = table.Column<bool>(type: "bit", nullable: false),
                    IndDomingo = table.Column<bool>(type: "bit", nullable: false),
                    LunDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    LunHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    MarDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    MarHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    MieDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    MieHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    JueDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    JueHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    VieDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    VieHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    SabDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    SabHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    DomDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    DomHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    IndComida = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ComidaDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    ComidaHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    TotalHorasSemana = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horario", x => x.IdHorario);
                    table.ForeignKey(
                        name: "FK_Horario_TipoHorario",
                        column: x => x.IdTipoHorario,
                        principalTable: "TipoHorario",
                        principalColumn: "IdTipoHorario");
                });

            migrationBuilder.CreateTable(
                name: "ClaseEmpleado",
                columns: table => new
                {
                    IdClaseEmpleado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreClaseEmpleado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    IdHorario = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaseEmpleado", x => x.IdClaseEmpleado);
                    table.ForeignKey(
                        name: "FK_ClaseEmpleado_Horario",
                        column: x => x.IdHorario,
                        principalTable: "Horario",
                        principalColumn: "IdHorario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Empleado",
                columns: table => new
                {
                    IdEmpleado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoInterno = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NombreEmpleado = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    ApellidoEmpleado = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    NumeroIdentidad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NumeroLicencia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaVencimientoLicencia = table.Column<DateOnly>(type: "date", nullable: true),
                    Nacionalidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Fotografia = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FotografiaNombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotografiaPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CiudadResidencia = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IdCargo = table.Column<int>(type: "int", nullable: false),
                    IdDepartamento = table.Column<int>(type: "int", nullable: false),
                    IdTipoContrato = table.Column<int>(type: "int", nullable: false),
                    IdTipoNomina = table.Column<int>(type: "int", nullable: false),
                    IdEncargado = table.Column<int>(type: "int", nullable: true),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: true),
                    IdBanco = table.Column<int>(type: "int", nullable: true),
                    CuentaBancaria = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NumeroRegistroTributario = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SalarioBase = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoCivil = table.Column<int>(type: "int", nullable: false),
                    FechaInactivacion = table.Column<DateOnly>(type: "date", nullable: true),
                    MotivoInactivacion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IdClaseEmpleado = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Empleado__CE6D8B9EC12710B3", x => x.IdEmpleado);
                    table.ForeignKey(
                        name: "FK_Empleado_Banco",
                        column: x => x.IdBanco,
                        principalTable: "Banco",
                        principalColumn: "IdBanco");
                    table.ForeignKey(
                        name: "FK_Empleado_Cargo",
                        column: x => x.IdCargo,
                        principalTable: "Cargo",
                        principalColumn: "IdCargo");
                    table.ForeignKey(
                        name: "FK_Empleado_ClaseEmpleado",
                        column: x => x.IdClaseEmpleado,
                        principalTable: "ClaseEmpleado",
                        principalColumn: "IdClaseEmpleado");
                    table.ForeignKey(
                        name: "FK_Empleado_Departamento",
                        column: x => x.IdDepartamento,
                        principalTable: "Departamento",
                        principalColumn: "IdDepartamento");
                    table.ForeignKey(
                        name: "FK_Empleado_EmpleadoEncargado",
                        column: x => x.IdEncargado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                    table.ForeignKey(
                        name: "FK_Empleado_TipoContrato",
                        column: x => x.IdTipoContrato,
                        principalTable: "TipoContrato",
                        principalColumn: "IdTipoContrato");
                    table.ForeignKey(
                        name: "FK_Empleado_TipoNomina",
                        column: x => x.IdTipoNomina,
                        principalTable: "TipoNomina",
                        principalColumn: "IdTipoNomina");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoActivo",
                columns: table => new
                {
                    IdEmpleadoActivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroSerie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "Nuevo/Usado/Reacondicionado"),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioEstimado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaAsignacion = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoActivo", x => x.IdEmpleadoActivo);
                    table.ForeignKey(
                        name: "FK_EmpleadoActivo_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                    table.ForeignKey(
                        name: "FK_EmpleadoActivo_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoAusencia",
                columns: table => new
                {
                    IdEmpleadoAusencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdTipoAusencia = table.Column<int>(type: "int", nullable: false),
                    DiaCompleto = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Estado = table.Column<int>(type: "int", nullable: false, comment: "Solicitada/Aprobada/Rechazada"),
                    FechaDesde = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaHasta = table.Column<DateOnly>(type: "date", nullable: false),
                    HoraDesde = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraHasta = table.Column<TimeOnly>(type: "time", nullable: false),
                    AprobadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoAusencia", x => x.IdEmpleadoAusencia);
                    table.ForeignKey(
                        name: "FK_EmpleadoAusencia_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                    table.ForeignKey(
                        name: "FK_EmpleadoAusencia_TipoAusencia",
                        column: x => x.IdTipoAusencia,
                        principalTable: "TipoAusencia",
                        principalColumn: "IdTipoAusencia");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoCargoHistorico",
                columns: table => new
                {
                    IdEmpleadoCargo = table.Column<int>(type: "int", nullable: false),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    IdCargo = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoCargoHistorico", x => x.IdEmpleadoCargo);
                    table.ForeignKey(
                        name: "FK_EmpleadoCargoHistorico_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoContacto",
                columns: table => new
                {
                    IdContactoEmergencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    NombreContacto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Relacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Cónyugue, Hermano, Primo, Amigo, Etc."),
                    Celular = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TelefonoFijo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoContacto", x => x.IdContactoEmergencia);
                    table.ForeignKey(
                        name: "FK_EmpleadoContacto_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoContrato",
                columns: table => new
                {
                    IdEmpleadoContrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    CodigoContrato = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdTipoContrato = table.Column<int>(type: "int", nullable: false),
                    IdCargo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    VigenciaMeses = table.Column<int>(type: "int", nullable: false, defaultValue: 6),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: false),
                    Salario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoContrato", x => x.IdEmpleadoContrato);
                    table.ForeignKey(
                        name: "FK_EmpleadoContrato_Cargo",
                        column: x => x.IdCargo,
                        principalTable: "Cargo",
                        principalColumn: "IdCargo");
                    table.ForeignKey(
                        name: "FK_EmpleadoContrato_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                    table.ForeignKey(
                        name: "FK_EmpleadoContrato_TipoContrato",
                        column: x => x.IdTipoContrato,
                        principalTable: "TipoContrato",
                        principalColumn: "IdTipoContrato");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoDeduccion",
                columns: table => new
                {
                    IdDeduccion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoDeduccion", x => new { x.IdDeduccion, x.IdEmpleado });
                    table.ForeignKey(
                        name: "FK_EmpleadoDeduccion_Deduccion",
                        column: x => x.IdDeduccion,
                        principalTable: "Deduccion",
                        principalColumn: "IdDeduccion");
                    table.ForeignKey(
                        name: "FK_EmpleadoDeduccion_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoEducacion",
                columns: table => new
                {
                    IdEmpleadoEducacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TituloObtenido = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    FechaDesde = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaHasta = table.Column<DateOnly>(type: "date", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoEducacion", x => x.IdEmpleadoEducacion);
                    table.ForeignKey(
                        name: "FK_EmpleadoEducacion_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoExperiencia",
                columns: table => new
                {
                    IdEmpleadoExperiencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    Empresa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    FechaDesde = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaHasta = table.Column<DateOnly>(type: "date", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoExperiencia", x => x.IdEmpleadoExperiencia);
                    table.ForeignKey(
                        name: "FK_EmpleadoExperiencia_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoHabilidad",
                columns: table => new
                {
                    IdEmpleadoHabilidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    Habilidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExperienciaYears = table.Column<int>(type: "int", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoHabilidad", x => x.IdEmpleadoHabilidad);
                    table.ForeignKey(
                        name: "FK_EmpleadoHabilidad_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoHorario",
                columns: table => new
                {
                    IdEmpleadoHorario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Idempleado = table.Column<int>(type: "int", nullable: false),
                    IdHorarioBase = table.Column<int>(type: "int", nullable: false),
                    IndLunes = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndMartes = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndMiercoles = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndJueves = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndViernes = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IndSabado = table.Column<bool>(type: "bit", nullable: false),
                    IndDomingo = table.Column<bool>(type: "bit", nullable: false),
                    LunDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    LunHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    MarDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    MarHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    MieDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    MieHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    JueDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    JueHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    VieDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    VieHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    SabDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    SabHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    DomDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    DomHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    IndComida = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ComidaDesde = table.Column<TimeOnly>(type: "time", nullable: true),
                    ComidaHasta = table.Column<TimeOnly>(type: "time", nullable: true),
                    TotalHorasSemana = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoTurno", x => x.IdEmpleadoHorario);
                    table.ForeignKey(
                        name: "FK_EmpleadoHorario_Empleado",
                        column: x => x.Idempleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                    table.ForeignKey(
                        name: "FK_EmpleadoHorario_Horario",
                        column: x => x.IdHorarioBase,
                        principalTable: "Horario",
                        principalColumn: "IdHorario");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoIngreso",
                columns: table => new
                {
                    IdIngreso = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoIngreso", x => new { x.IdIngreso, x.IdEmpleado });
                    table.ForeignKey(
                        name: "FK_EmpleadoIngreso_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                    table.ForeignKey(
                        name: "FK_EmpleadoIngreso_Ingreso",
                        column: x => x.IdIngreso,
                        principalTable: "Ingreso",
                        principalColumn: "IdIngreso");
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoSalarioHistorico",
                columns: table => new
                {
                    IdEmpleadoSalarioHistorico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpleado = table.Column<int>(type: "int", nullable: false),
                    SalarioAnterior = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SalarioActual = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    CreadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS"),
                    ModificadoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Admin", collation: "SQL_Latin1_General_CP1_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoSalarioHistorico", x => x.IdEmpleadoSalarioHistorico);
                    table.ForeignKey(
                        name: "FK_EmpleadoSalarioHistorico_Empleado",
                        column: x => x.IdEmpleado,
                        principalTable: "Empleado",
                        principalColumn: "IdEmpleado");
                });

            migrationBuilder.CreateTable(
              name: "Ventana",
              columns: table => new
              {
                  Id = table.Column<int>(nullable: false)
                      .Annotation("SqlServer:Identity", "1, 1"),
                  Nombre = table.Column<string>(nullable: false),
                  Activo = table.Column<bool>(nullable: false),
                  FechaCreacion = table.Column<DateTime>(nullable: false),
                  FechaModificacion = table.Column<DateTime>(nullable: true),
                  CreadoPor = table.Column<string>(nullable: false),
                  ModificadoPor = table.Column<string>(nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Ventana", x => x.Id);
              });

            migrationBuilder.CreateTable(
                name: "RoleVentana",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    VentanaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleVentana", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleVentana_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleVentana_Ventana_VentanaId",
                        column: x => x.VentanaId,
                        principalTable: "Ventana",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleVentana_RoleId",
                table: "RoleVentana",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleVentana_VentanaId",
                table: "RoleVentana",
                column: "VentanaId");
        

        migrationBuilder.CreateIndex(
                name: "IX_RoleVentanas_RoleId",
                table: "RoleVentanas",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleVentanas_VentanaId",
                table: "RoleVentanas",
                column: "VentanaId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaseEmpleado_IdHorario",
                table: "ClaseEmpleado",
                column: "IdHorario");

            migrationBuilder.CreateIndex(
                name: "IX_Departamento_IdDivision",
                table: "Departamento",
                column: "IdDivision");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_IdBanco",
                table: "Empleado",
                column: "IdBanco");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_IdCargo",
                table: "Empleado",
                column: "IdCargo");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_IdClaseEmpleado",
                table: "Empleado",
                column: "IdClaseEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_IdDepartamento",
                table: "Empleado",
                column: "IdDepartamento");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_IdEncargado",
                table: "Empleado",
                column: "IdEncargado");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_IdTipoContrato",
                table: "Empleado",
                column: "IdTipoContrato");

            migrationBuilder.CreateIndex(
                name: "IX_Empleado_IdTipoNomina",
                table: "Empleado",
                column: "IdTipoNomina");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoActivo_IdEmpleado",
                table: "EmpleadoActivo",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoActivo_IdProducto",
                table: "EmpleadoActivo",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoAusencia_IdEmpleado",
                table: "EmpleadoAusencia",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoAusencia_IdTipoAusencia",
                table: "EmpleadoAusencia",
                column: "IdTipoAusencia");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoCargoHistorico_IdEmpleado",
                table: "EmpleadoCargoHistorico",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoContacto_IdEmpleado",
                table: "EmpleadoContacto",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoContrato_IdCargo",
                table: "EmpleadoContrato",
                column: "IdCargo");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoContrato_IdEmpleado",
                table: "EmpleadoContrato",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoContrato_IdTipoContrato",
                table: "EmpleadoContrato",
                column: "IdTipoContrato");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoDeduccion_IdEmpleado",
                table: "EmpleadoDeduccion",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoEducacion_IdEmpleado",
                table: "EmpleadoEducacion",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoExperiencia_IdEmpleado",
                table: "EmpleadoExperiencia",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoHabilidad_IdEmpleado",
                table: "EmpleadoHabilidad",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoHorario_Idempleado",
                table: "EmpleadoHorario",
                column: "Idempleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoHorario_IdHorarioBase",
                table: "EmpleadoHorario",
                column: "IdHorarioBase");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoIngreso_IdEmpleado",
                table: "EmpleadoIngreso",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoSalarioHistorico_IdEmpleado",
                table: "EmpleadoSalarioHistorico",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_IdMoneda",
                table: "Empresa",
                column: "IdMoneda");

            migrationBuilder.CreateIndex(
                name: "IX_Horario_IdTipoHorario",
                table: "Horario",
                column: "IdTipoHorario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivoAdjunto");

            migrationBuilder.DropTable(
                name: "DiaFestivo");

            migrationBuilder.DropTable(
                name: "EmpleadoActivo");

            migrationBuilder.DropTable(
                name: "EmpleadoAusencia");

            migrationBuilder.DropTable(
                name: "EmpleadoCargoHistorico");

            migrationBuilder.DropTable(
                name: "EmpleadoContacto");

            migrationBuilder.DropTable(
                name: "EmpleadoContrato");

            migrationBuilder.DropTable(
                name: "EmpleadoDeduccion");

            migrationBuilder.DropTable(
                name: "EmpleadoEducacion");

            migrationBuilder.DropTable(
                name: "EmpleadoExperiencia");

            migrationBuilder.DropTable(
                name: "EmpleadoHabilidad");

            migrationBuilder.DropTable(
                name: "EmpleadoHorario");

            migrationBuilder.DropTable(
                name: "EmpleadoIngreso");

            migrationBuilder.DropTable(
                name: "EmpleadoSalarioHistorico");

            migrationBuilder.DropTable(
                name: "Empresa");

            migrationBuilder.DropTable(
                name: "Impuesto");

            migrationBuilder.DropTable(
                name: "ImpuestoTabla");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "TipoAusencia");

            migrationBuilder.DropTable(
                name: "Deduccion");

            migrationBuilder.DropTable(
                name: "Ingreso");

            migrationBuilder.DropTable(
                name: "Empleado");

            migrationBuilder.DropTable(
                name: "Moneda");

            migrationBuilder.DropTable(
                name: "Banco");

            migrationBuilder.DropTable(
                name: "Cargo");

            migrationBuilder.DropTable(
                name: "ClaseEmpleado");

            migrationBuilder.DropTable(
                name: "Departamento");

            migrationBuilder.DropTable(
                name: "TipoContrato");

            migrationBuilder.DropTable(
                name: "TipoNomina");

            migrationBuilder.DropTable(
                name: "Horario");

            migrationBuilder.DropTable(
                name: "Division");

            migrationBuilder.DropTable(
                name: "TipoHorario");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AvatarName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.DropTable(
           name: "RoleVentana");

            migrationBuilder.DropTable(
                name: "Ventana");
        }
    }
}
