using InovaSaude.Application.DTOs;
using InovaSaude.Core.Entities;
using InovaSaude.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InovaSaude.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UbsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UbsController> _logger;

    public UbsController(IUnitOfWork unitOfWork, ILogger<UbsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UbsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var ubsList = await _unitOfWork.Ubs.GetAllAsync();
            var dtos = ubsList.Select(u => new UbsDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Endereco = u.Endereco,
                Telefone = u.Telefone,
                Cnes = u.Cnes,
                MunicipioId = u.MunicipioId,
                MunicipioNome = u.Municipio.Nome,
                CreatedAt = u.CreatedAt
            });
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving UBS list");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UbsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var ubs = await _unitOfWork.Ubs.GetByIdAsync(id);
            if (ubs == null)
                return NotFound(new { message = "UBS not found" });

            var dto = new UbsDto
            {
                Id = ubs.Id,
                Nome = ubs.Nome,
                Endereco = ubs.Endereco,
                Telefone = ubs.Telefone,
                Cnes = ubs.Cnes,
                MunicipioId = ubs.MunicipioId,
                MunicipioNome = ubs.Municipio.Nome,
                CreatedAt = ubs.CreatedAt
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving UBS {Id}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Gestor")]
    [ProducesResponseType(typeof(UbsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUbsDto dto)
    {
        try
        {
            var ubs = new UBS
            {
                Nome = dto.Nome,
                Endereco = dto.Endereco,
                Telefone = dto.Telefone,
                Cnes = dto.Cnes,
                MunicipioId = dto.MunicipioId
            };

            await _unitOfWork.Ubs.AddAsync(ubs);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("UBS {Nome} created successfully", ubs.Nome);

            // Fetch the created UBS with Municipio for response
            var createdUbs = await _unitOfWork.Ubs.GetByIdAsync(ubs.Id);
            var responseDto = new UbsDto
            {
                Id = createdUbs!.Id,
                Nome = createdUbs.Nome,
                Endereco = createdUbs.Endereco,
                Telefone = createdUbs.Telefone,
                Cnes = createdUbs.Cnes,
                MunicipioId = createdUbs.MunicipioId,
                MunicipioNome = createdUbs.Municipio.Nome,
                CreatedAt = createdUbs.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = ubs.Id }, responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating UBS");
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Gestor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUbsDto dto)
    {
        try
        {
            var ubs = await _unitOfWork.Ubs.GetByIdAsync(id);
            if (ubs == null)
                return NotFound(new { message = "UBS not found" });

            if (!string.IsNullOrEmpty(dto.Nome))
                ubs.Nome = dto.Nome;
            if (dto.Endereco != null)
                ubs.Endereco = dto.Endereco;
            if (dto.Telefone != null)
                ubs.Telefone = dto.Telefone;
            if (dto.Cnes != null)
                ubs.Cnes = dto.Cnes;

            await _unitOfWork.Ubs.UpdateAsync(ubs);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("UBS {Id} updated successfully", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating UBS {Id}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var ubs = await _unitOfWork.Ubs.GetByIdAsync(id);
            if (ubs == null)
                return NotFound(new { message = "UBS not found" });

            await _unitOfWork.Ubs.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("UBS {Id} deleted successfully", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting UBS {Id}", id);
            return StatusCode(500, new { message = "An error occurred" });
        }
    }
}
