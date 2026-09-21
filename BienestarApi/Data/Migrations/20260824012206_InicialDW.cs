using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BienestarApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class InicialDW : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DimTiempo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Semana = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimTiempo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DimUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoUniversitario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Carrera = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimUsuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuentas_DimUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "DimUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncuestasBasal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    PuntajePHQ9 = table.Column<int>(type: "int", nullable: false),
                    PuntajeSISCO = table.Column<int>(type: "int", nullable: false),
                    PuntajeIPAQ = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fase = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncuestasBasal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncuestasBasal_DimUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "DimUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncuestasSUS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    PuntajeSUS = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncuestasSUS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncuestasSUS_DimUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "DimUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosDiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TiempoId = table.Column<int>(type: "int", nullable: false),
                    NivelEstres = table.Column<int>(type: "int", nullable: false),
                    CalidadSueno = table.Column<int>(type: "int", nullable: false),
                    MinutosActividadFisica = table.Column<int>(type: "int", nullable: false),
                    EstadoAnimo = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosDiarios_DimTiempo_TiempoId",
                        column: x => x.TiempoId,
                        principalTable: "DimTiempo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosDiarios_DimUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "DimUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertasRiesgo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EncuestaBasalId = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SeMostroMensajeAyuda = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasRiesgo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertasRiesgo_DimUsuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "DimUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertasRiesgo_EncuestasBasal_EncuestaBasalId",
                        column: x => x.EncuestaBasalId,
                        principalTable: "EncuestasBasal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertasRiesgo_EncuestaBasalId",
                table: "AlertasRiesgo",
                column: "EncuestaBasalId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertasRiesgo_UsuarioId",
                table: "AlertasRiesgo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_Email",
                table: "Cuentas",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_UsuarioId",
                table: "Cuentas",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DimTiempo_Fecha",
                table: "DimTiempo",
                column: "Fecha",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DimUsuario_CodigoUniversitario",
                table: "DimUsuario",
                column: "CodigoUniversitario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasBasal_UsuarioId_Fase",
                table: "EncuestasBasal",
                columns: new[] { "UsuarioId", "Fase" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasSUS_UsuarioId",
                table: "EncuestasSUS",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDiarios_TiempoId",
                table: "RegistrosDiarios",
                column: "TiempoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDiarios_UsuarioId_TiempoId",
                table: "RegistrosDiarios",
                columns: new[] { "UsuarioId", "TiempoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertasRiesgo");

            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "EncuestasSUS");

            migrationBuilder.DropTable(
                name: "RegistrosDiarios");

            migrationBuilder.DropTable(
                name: "EncuestasBasal");

            migrationBuilder.DropTable(
                name: "DimTiempo");

            migrationBuilder.DropTable(
                name: "DimUsuario");
        }
    }
}
