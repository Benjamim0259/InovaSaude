using InovaSaude.Core.Enums;

namespace InovaSaude.Core.Entities;

public class Despesa : BaseEntity
{
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DespesaStatus Status { get; set; } = DespesaStatus.Pendente;
    public string? ComprovanteUrl { get; set; }
    public string? Observacoes { get; set; }
    
    // Relacionamentos
    public Guid UbsId { get; set; }
    public UBS Ubs { get; set; } = null!;
    
    public Guid CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
