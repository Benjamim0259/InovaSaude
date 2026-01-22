using ClosedXML.Excel;
using CsvHelper;
using System.Globalization;

namespace InovaSaude.Web.Services;

public class ExportService : IExportService
{
    public byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);
        
        var properties = typeof(T).GetProperties();
        
        // Headers
        for (int i = 0; i < properties.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = properties[i].Name;
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
        }
        
        // Data
        int row = 2;
        foreach (var item in data)
        {
            for (int col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(item);
                worksheet.Cell(row, col + 1).Value = value?.ToString() ?? "";
            }
            row++;
        }
        
        worksheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.GetBuffer()[..(int)stream.Length];
    }

    public byte[] ExportToCsv<T>(IEnumerable<T> data)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteRecords(data);
        writer.Flush();
        stream.Position = 0;
        return stream.GetBuffer()[..(int)stream.Length];
    }
}
