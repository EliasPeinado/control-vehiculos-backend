using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlVehiculos.Migrations
{
    /// <inheritdoc />
    public partial class AddCreadoPorUsuarioToTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreadoPorUsuarioId",
                table: "Turnos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_CreadoPorUsuarioId",
                table: "Turnos",
                column: "CreadoPorUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Usuarios_CreadoPorUsuarioId",
                table: "Turnos",
                column: "CreadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Usuarios_CreadoPorUsuarioId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_CreadoPorUsuarioId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "CreadoPorUsuarioId",
                table: "Turnos");
        }
    }
}
