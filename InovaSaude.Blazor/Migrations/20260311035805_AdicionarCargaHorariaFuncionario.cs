using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InovaSaude.Blazor.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCargaHorariaFuncionario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_esf_Codigo",
                table: "esf");

            migrationBuilder.DropColumn(
                name: "Cnes",
                table: "esf");

            migrationBuilder.AddColumn<int>(
                name: "CargaHoraria",
                table: "funcionarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "esf",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_esf_Codigo",
                table: "esf",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_esf_Codigo",
                table: "esf");

            migrationBuilder.DropColumn(
                name: "CargaHoraria",
                table: "funcionarios");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "esf",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Cnes",
                table: "esf",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_esf_Codigo",
                table: "esf",
                column: "Codigo",
                unique: true,
                filter: "[Codigo] IS NOT NULL");
        }
    }
}
