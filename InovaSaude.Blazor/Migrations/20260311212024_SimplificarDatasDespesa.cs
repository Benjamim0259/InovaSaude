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
            var isPostgres = migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL";

            if (isPostgres)
            {
                // PostgreSQL: usar SQL seguro com verificação de existência
                migrationBuilder.Sql(@"
                    DO $$ BEGIN
                        IF EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'IX_despesas_DataVencimento') THEN
                            DROP INDEX ""IX_despesas_DataVencimento"";
                        END IF;
                    END $$;
                ");

                migrationBuilder.Sql(@"
                    DO $$ BEGIN
                        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='funcionarios' AND column_name='CargaHoraria') THEN
                            ALTER TABLE funcionarios DROP COLUMN ""CargaHoraria"";
                        END IF;
                        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='despesas' AND column_name='DataPagamento') THEN
                            ALTER TABLE despesas DROP COLUMN ""DataPagamento"";
                        END IF;
                        IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='despesas' AND column_name='DataVencimento') THEN
                            ALTER TABLE despesas DROP COLUMN ""DataVencimento"";
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='despesas' AND column_name='MesReferencia') THEN
                            ALTER TABLE despesas ADD COLUMN ""MesReferencia"" timestamp with time zone NOT NULL DEFAULT NOW();
                        END IF;
                        IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = 'IX_despesas_MesReferencia') THEN
                            CREATE INDEX ""IX_despesas_MesReferencia"" ON despesas (""MesReferencia"");
                        END IF;
                    END $$;
                ");
            }
            else
            {
                // SQL Server
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
                    defaultValue: DateTime.UtcNow);

                migrationBuilder.CreateIndex(
                    name: "IX_despesas_MesReferencia",
                    table: "despesas",
                    column: "MesReferencia");
            }
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
