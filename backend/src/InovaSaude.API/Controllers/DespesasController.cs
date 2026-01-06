using InovaSaude.Application.DTOs;
using InovaSaude.Core.Entities;
using InovaSaude.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InovaSaude.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DespesasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DespesasController> _logger;

    public DespesasController(IUnitOfWork unitOfWork, ILogger<DespesasController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DespesaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? ubsId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        try
        {
            IEnumerable<Despesa> despesas;

            if (ubsId.HasValue && dataInicio.HasValue && dataFim.HasValue)
            {
                despesas = await _unitOfWork.Despesas.GetByUbsAndPeriodAsync(ubsId.Value, dataInicio.Value, dataFim.Value);
            }
            else if (ubsId.HasValue)
            {
                despesas = await _unitOfWork.Despesas.GetByUbsIdAsync(ubsId.Value);
            }
            else if (dataInicio.HasValue && dataFim.HasValue)
            {
                despesas = await _unitOfWork.Despesas.GetByPeriodAsync(dataInicio.Value, dataFim.Value);
            }
            else
            {
                despesas = await _unitOfWork.Despesas.GetAllAsync();
            }

            var dtos = despesas.Select(d => MapToDespesaDto(d));
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving despesas");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DespesaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var despesa = await _unitOfWork.Despesas.GetByIdAsync(id);
            if (despesa == null)
                return NotFound(new { message = "Despesa not found" });

            return Ok(MapToDespesaDto(despesa));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving despesa {Id}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Gestor,Coordenador")]
    [ProducesResponseType(typeof(DespesaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDespesaDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var despesa = new Despesa
            {
                Valor = dto.Valor,
                Data = dto.Data,
                Descricao = dto.Descricao,
                Observacoes = dto.Observacoes,
                UbsId = dto.UbsId,
                CategoriaId = dto.CategoriaId,
                UsuarioId = userId
            };

            await _unitOfWork.Despesas.AddAsync(despesa);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Despesa created successfully by user {UserId}", userId);

            var createdDespesa = await _unitOfWork.Despesas.GetByIdAsync(despesa.Id);
            return CreatedAtAction(nameof(GetById), new { id = despesa.Id }, MapToDespesaDto(createdDespesa!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating despesa");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Gestor,Coordenador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDespesaDto dto)
    {
        try
        {
            var despesa = await _unitOfWork.Despesas.GetByIdAsync(id);
            if (despesa == null)
                return NotFound(new { message = "Despesa not found" });

            if (dto.Valor.HasValue)
                despesa.Valor = dto.Valor.Value;
            if (dto.Data.HasValue)
                despesa.Data = dto.Data.Value;
            if (!string.IsNullOrEmpty(dto.Descricao))
                despesa.Descricao = dto.Descricao;
            if (dto.Observacoes != null)
                despesa.Observacoes = dto.Observacoes;
            if (dto.Status.HasValue)
                despesa.Status = dto.Status.Value;
            if (dto.CategoriaId.HasValue)
                despesa.CategoriaId = dto.CategoriaId.Value;

            await _unitOfWork.Despesas.UpdateAsync(despesa);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Despesa {Id} updated successfully", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating despesa {Id}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Gestor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var despesa = await _unitOfWork.Despesas.GetByIdAsync(id);
            if (despesa == null)
                return NotFound(new { message = "Despesa not found" });

            await _unitOfWork.Despesas.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Despesa {Id} deleted successfully", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting despesa {Id}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPost("{id}/comprovante")]
    [Authorize(Roles = "Admin,Gestor,Coordenador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadComprovante(Guid id, IFormFile file)
    {
        try
        {
            var despesa = await _unitOfWork.Despesas.GetByIdAsync(id);
            if (despesa == null)
                return NotFound(new { message = "Despesa not found" });

            // Simple file upload implementation
            // In production, use cloud storage (Azure Blob, AWS S3, etc.)
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            despesa.ComprovanteUrl = $"/uploads/{fileName}";
            await _unitOfWork.Despesas.UpdateAsync(despesa);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Comprovante uploaded for despesa {Id}", id);

            return Ok(new { url = despesa.ComprovanteUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading comprovante for despesa {Id}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    private static DespesaDto MapToDespesaDto(Despesa despesa)
    {
        return new DespesaDto
        {
            Id = despesa.Id,
            Valor = despesa.Valor,
            Data = despesa.Data,
            Descricao = despesa.Descricao,
            Status = despesa.Status,
            ComprovanteUrl = despesa.ComprovanteUrl,
            Observacoes = despesa.Observacoes,
            UbsId = despesa.UbsId,
            UbsNome = despesa.Ubs.Nome,
            CategoriaId = despesa.CategoriaId,
            CategoriaNome = despesa.Categoria.Nome,
            UsuarioId = despesa.UsuarioId,
            UsuarioNome = despesa.Usuario.Nome,
            CreatedAt = despesa.CreatedAt
        };
    }
}
