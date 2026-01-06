using Microsoft.AspNetCore.Identity;

namespace InovaSaude.Core.Entities;

public class Usuario : IdentityUser<Guid>
{
    public string Nome { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    // Relacionamentos
    public Guid? MunicipioId { get; set; }
    public Municipio? Municipio { get; set; }
    
    public Guid? UbsId { get; set; }
    public UBS? Ubs { get; set; }
    
    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
}
