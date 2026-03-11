namespace InovaSaude.Blazor.Helpers;

public static class DateTimeHelper
{
    private static readonly TimeZoneInfo BrasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

    /// <summary>
    /// Converte UTC para horário de Brasília
    /// </summary>
    public static DateTime ToBrasilia(this DateTime utcDateTime)
    {
        if (utcDateTime.Kind == DateTimeKind.Unspecified)
            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, BrasiliaTimeZone);
    }

    /// <summary>
    /// Retorna o DateTime atual no horário de Brasília
    /// </summary>
    public static DateTime NowBrasilia()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BrasiliaTimeZone);
    }

    /// <summary>
    /// Formata data no padrão brasileiro
    /// </summary>
    public static string FormatarDataBR(this DateTime? data)
    {
        if (!data.HasValue)
            return "-";

        return data.Value.ToBrasilia().ToString("dd/MM/yyyy HH:mm");
    }

    /// <summary>
    /// Formata data no padrão brasileiro (apenas data)
    /// </summary>
    public static string FormatarDataCurtaBR(this DateTime? data)
    {
        if (!data.HasValue)
            return "-";

        return data.Value.ToBrasilia().ToString("dd/MM/yyyy");
    }
}
