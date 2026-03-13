using InovaSaude.Blazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InovaSaude.Blazor.Controllers;

[ApiController]
[Route("api/backup")]
[Authorize(Roles = "ADMIN")]
public class BackupController : ControllerBase
{
    private readonly BackupService _backupService;

    public BackupController(BackupService backupService)
    {
        _backupService = backupService;
    }

    [HttpGet("exportar")]
    public async Task<IActionResult> Exportar()
    {
        var backup = await _backupService.ExportarAsync();
        var json = _backupService.SerializarBackup(backup);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var fileName = $"inovasaude_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
        return File(bytes, "application/json", fileName);
    }

    [HttpPost("importar")]
    [RequestSizeLimit(50_000_000)] // 50MB max
    public async Task<IActionResult> Importar(IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest(new { message = "Arquivo vazio" });

        using var reader = new StreamReader(arquivo.OpenReadStream());
        var json = await reader.ReadToEndAsync();

        var backup = _backupService.DesserializarBackup(json);
        if (backup == null)
            return BadRequest(new { message = "Arquivo de backup invalido" });

        var result = await _backupService.ImportarAsync(backup);
        return Ok(result);
    }
}
