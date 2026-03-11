using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InovaSaude.Blazor.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposSegurancaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CargaHoraria",
                table: "funcionarios");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataBloqueio",
                table: "usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimaTrocaSenha",
                table: "usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TentativasLoginFalhas",
                table: "usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataBloqueio",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "DataUltimaTrocaSenha",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "TentativasLoginFalhas",
                table: "usuarios");

            migrationBuilder.AddColumn<int>(
                name: "CargaHoraria",
                table: "funcionarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
