using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BienestarApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregaRespuestaItem9Phq9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RespuestaItem9",
                table: "EncuestasBasal",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RespuestaItem9",
                table: "EncuestasBasal");
        }
    }
}
