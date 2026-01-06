namespace InovaSaude.Core.Entities;

public class UBS : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public string? Cnes { get; set; } // Código Nacional de Estabelecimento de Saúde
    
    // Relacionamentos
    public Guid MunicipioId { get; set; }
    public Municipio Municipio { get; set; } = null!;
    
    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
    public ICollection<Usuario> Coordenadores { get; set; } = new List<Usuario>();
}
