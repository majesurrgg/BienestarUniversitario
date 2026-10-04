using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BienestarApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregaConsentimientoYSusDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EncuestasSUS_UsuarioId",
                table: "EncuestasSUS");

            migrationBuilder.AddColumn<string>(
                name: "ComentarioLoMasUtil",
                table: "EncuestasSUS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentarioMejoras",
                table: "EncuestasSUS",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RespuestasSUS",
                table: "EncuestasSUS",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAceptaConsentimiento",
                table: "DimUsuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VersionConsentimiento",
                table: "DimUsuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasSUS_UsuarioId",
                table: "EncuestasSUS",
                column: "UsuarioId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EncuestasSUS_UsuarioId",
                table: "EncuestasSUS");

            migrationBuilder.DropColumn(
                name: "ComentarioLoMasUtil",
                table: "EncuestasSUS");

            migrationBuilder.DropColumn(
                name: "ComentarioMejoras",
                table: "EncuestasSUS");

            migrationBuilder.DropColumn(
                name: "RespuestasSUS",
                table: "EncuestasSUS");

            migrationBuilder.DropColumn(
                name: "FechaAceptaConsentimiento",
                table: "DimUsuario");

            migrationBuilder.DropColumn(
                name: "VersionConsentimiento",
                table: "DimUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_EncuestasSUS_UsuarioId",
                table: "EncuestasSUS",
                column: "UsuarioId");
        }
    }
}
