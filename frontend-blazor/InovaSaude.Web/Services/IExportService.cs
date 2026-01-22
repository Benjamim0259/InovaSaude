namespace InovaSaude.Web.Services;

public interface IExportService
{
    byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName);
    byte[] ExportToCsv<T>(IEnumerable<T> data);
}
