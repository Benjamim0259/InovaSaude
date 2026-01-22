namespace InovaSaude.Web.Models;

public class DespesaDto
{
    public Guid Id { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DespesaStatus Status { get; set; }
    public string? ComprovanteUrl { get; set; }
    public string? Observacoes { get; set; }
    public Guid UbsId { get; set; }
    public string UbsNome { get; set; } = string.Empty;
    public Guid CategoriaId { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateDespesaDto
{
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public Guid UbsId { get; set; }
    public Guid CategoriaId { get; set; }
}

public class UpdateDespesaDto
{
    public decimal? Valor { get; set; }
    public DateTime? Data { get; set; }
    public string? Descricao { get; set; }
    public string? Observacoes { get; set; }
    public DespesaStatus? Status { get; set; }
    public Guid? CategoriaId { get; set; }
}
