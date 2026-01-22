namespace InovaSaude.Web.Models;

public class UbsDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public string? Cnes { get; set; }
    public Guid MunicipioId { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateUbsDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public string? Cnes { get; set; }
    public Guid MunicipioId { get; set; }
}

public class UpdateUbsDto
{
    public string? Nome { get; set; }
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }
    public string? Cnes { get; set; }
}
