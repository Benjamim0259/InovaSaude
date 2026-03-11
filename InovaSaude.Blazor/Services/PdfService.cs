using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InovaSaude.Blazor.Models;
using System.Globalization;

namespace InovaSaude.Blazor.Services;

public class PdfService
{
    public byte[] GerarRelatorioPDF(
        List<Despesa> despesas,
        List<Funcionario> funcionarios,
        DateTime dataInicio,
        DateTime dataFim,
        decimal totalDespesas,
        decimal totalSalarios)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .AlignCenter()
                    .Column(column =>
                    {
                        column.Item().Text("InovaSaúde - Relatório Financeiro")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.Blue.Medium);

                        column.Item().Text($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}")
                            .FontSize(12)
                            .FontColor(Colors.Grey.Darken2);

                        column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Blue.Medium);
                    });

                page.Content()
                    .PaddingVertical(10)
                    .Column(column =>
                    {
                        // Resumo Executivo
                        column.Item().Text("📊 Resumo Executivo").FontSize(16).Bold();
                        column.Item().PaddingVertical(5).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Total Despesas:").Bold();
                                col.Item().Text(totalDespesas.ToString("C", new CultureInfo("pt-BR")))
                                    .FontSize(14).FontColor(Colors.Red.Medium);
                            });
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Total Salários:").Bold();
                                col.Item().Text(totalSalarios.ToString("C", new CultureInfo("pt-BR")))
                                    .FontSize(14).FontColor(Colors.Orange.Medium);
                            });
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Total Geral:").Bold();
                                col.Item().Text((totalDespesas + totalSalarios).ToString("C", new CultureInfo("pt-BR")))
                                    .FontSize(14).FontColor(Colors.Blue.Medium);
                            });
                        });

                        column.Item().PaddingTop(15).Text("💰 Despesas Detalhadas").FontSize(14).Bold();
                        column.Item().PaddingVertical(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Data").Bold();
                                header.Cell().Element(CellStyle).Text("Descrição").Bold();
                                header.Cell().Element(CellStyle).Text("Categoria").Bold();
                                header.Cell().Element(CellStyle).Text("Valor").Bold();
                            });

                            foreach (var despesa in despesas.OrderBy(d => d.CreatedAt).Take(50))
                            {
                                table.Cell().Element(CellStyle).Text(despesa.CreatedAt.ToString("dd/MM/yyyy"));
                                table.Cell().Element(CellStyle).Text(despesa.Descricao);
                                table.Cell().Element(CellStyle).Text(despesa.Categoria?.Nome ?? "-");
                                table.Cell().Element(CellStyle).Text(despesa.Valor.ToString("C", new CultureInfo("pt-BR")));
                            }
                        });

                        if (despesas.Count > 50)
                        {
                            column.Item().PaddingTop(5).Text($"... e mais {despesas.Count - 50} despesas")
                                .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                        }

                        column.Item().PaddingTop(15).Text("👥 Funcionários").FontSize(14).Bold();
                        column.Item().PaddingVertical(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Nome").Bold();
                                header.Cell().Element(CellStyle).Text("Cargo").Bold();
                                header.Cell().Element(CellStyle).Text("Salário").Bold();
                                header.Cell().Element(CellStyle).Text("CH").Bold();
                            });

                            foreach (var func in funcionarios.OrderBy(f => f.Nome))
                            {
                                table.Cell().Element(CellStyle).Text(func.Nome);
                                table.Cell().Element(CellStyle).Text(func.Cargo ?? "-");
                                table.Cell().Element(CellStyle).Text(func.Salario.ToString("C", new CultureInfo("pt-BR")));
                                table.Cell().Element(CellStyle).Text($"{func.CargaHoraria}h");
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                        x.Span(" - Gerado em: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5);
    }
}
