using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InovaSaude.Blazor.Migrations
{
    /// <inheritdoc />
    public partial class SimplificarDatasDespesa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_despesas_DataVencimento",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "CargaHoraria",
                table: "funcionarios");

            migrationBuilder.DropColumn(
                name: "DataPagamento",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "DataVencimento",
                table: "despesas");

            migrationBuilder.AddColumn<DateTime>(
                name: "MesReferencia",
                table: "despesas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_despesas_MesReferencia",
                table: "despesas",
                column: "MesReferencia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_despesas_MesReferencia",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "MesReferencia",
                table: "despesas");

            migrationBuilder.AddColumn<int>(
                name: "CargaHoraria",
                table: "funcionarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPagamento",
                table: "despesas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataVencimento",
                table: "despesas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_despesas_DataVencimento",
                table: "despesas",
                column: "DataVencimento");
        }
    }
}
