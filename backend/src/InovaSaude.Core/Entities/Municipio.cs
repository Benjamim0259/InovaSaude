namespace InovaSaude.Core.Entities;

public class Municipio : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? CodigoIbge { get; set; }
    
    // Relacionamentos
    public ICollection<UBS> UbsList { get; set; } = new List<UBS>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
