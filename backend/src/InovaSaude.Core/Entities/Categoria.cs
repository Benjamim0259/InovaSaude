namespace InovaSaude.Core.Entities;

public class Categoria : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Codigo { get; set; }
    
    // Relacionamentos
    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
}
