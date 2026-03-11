using InovaSaude.Blazor.Models;
using InovaSaude.Blazor.Services;

namespace InovaSaude.Blazor.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuditService auditService)
    {
        // Registrar ações relevantes
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // Ignorar recursos estáticos e _blazor
        if (path.Contains("_blazor") || path.Contains(".css") || 
            path.Contains(".js") || path.Contains(".ico"))
        {
            await _next(context);
            return;
        }

        var userId = context.User?.FindFirst("Id")?.Value;
        var userName = context.User?.FindFirst("Nome")?.Value;
        
        // Registrar antes da execução
        var startTime = DateTime.UtcNow;

        await _next(context);

        // Registrar após a execução
        var endTime = DateTime.UtcNow;
        var duration = (endTime - startTime).TotalMilliseconds;

        // Registrar apenas operações importantes
        if (path.Contains("gerenciar") || path.Contains("deletar") || 
            path.Contains("criar") || path.Contains("editar") ||
            context.Request.Method != "GET")
        {
            await auditService.LogActivityAsync(
                action: $"{context.Request.Method} {path}",
                entityType: ExtrairTipoEntidade(path),
                entityId: null,
                userId: userId,
                userEmail: context.User?.FindFirst("Email")?.Value,
                userName: userName,
                oldValues: null,
                newValues: null,
                changes: $"Status: {context.Response.StatusCode}, Duration: {duration}ms",
                ipAddress: context.Connection.RemoteIpAddress?.ToString(),
                userAgent: context.Request.Headers["User-Agent"].ToString(),
                sessionId: context.Session.Id,
                severity: context.Response.StatusCode >= 400 ? "HIGH" : "LOW"
            );
        }
    }

    private string ExtrairTipoEntidade(string path)
    {
        if (path.Contains("usuario")) return "Usuario";
        if (path.Contains("despesa")) return "Despesa";
        if (path.Contains("esf")) return "ESF";
        if (path.Contains("funcionario")) return "Funcionario";
        if (path.Contains("categoria")) return "Categoria";
        if (path.Contains("fornecedor")) return "Fornecedor";
        if (path.Contains("relatorio")) return "Relatorio";
        return "Sistema";
    }
}
