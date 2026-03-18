using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TelMedAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateConsulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "consulta",
                columns: table => new
                {
                    id_consulta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cita_id = table.Column<int>(type: "integer", nullable: false),
                    sintomas = table.Column<string>(type: "text", nullable: false),
                    evolucion = table.Column<string>(type: "text", nullable: false),
                    diagnostico = table.Column<string>(type: "text", nullable: false),
                    tratamiento = table.Column<string>(type: "text", nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: false),
                    medicamentos_json = table.Column<string>(type: "text", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consulta", x => x.id_consulta);
                    table.ForeignKey(
                        name: "FK_consulta_cita_cita_id",
                        column: x => x.cita_id,
                        principalTable: "cita",
                        principalColumn: "id_cita",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_consulta_cita_id",
                table: "consulta",
                column: "cita_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consulta");
        }
    }
}
