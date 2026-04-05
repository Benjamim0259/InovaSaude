namespace InovaSaude.Web.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public Guid? MunicipioId { get; set; }
    public string? MunicipioNome { get; set; }
    public Guid? UbsId { get; set; }
    public string? UbsNome { get; set; }
}

public class CreateUserDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string Role { get; set; } = string.Empty;
    public Guid? MunicipioId { get; set; }
    public Guid? UbsId { get; set; }
}

public class UpdateUserDto
{
    public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public Guid? MunicipioId { get; set; }
    public Guid? UbsId { get; set; }
}
