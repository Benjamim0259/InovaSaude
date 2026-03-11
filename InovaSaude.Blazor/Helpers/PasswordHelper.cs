namespace InovaSaude.Blazor.Helpers;

public static class PasswordHelper
{
    public static bool ValidarSenhaForte(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            return false;

        // Mínimo 8 caracteres
        if (senha.Length < 8)
            return false;

        // Pelo menos uma letra maiúscula
        if (!senha.Any(char.IsUpper))
            return false;

        // Pelo menos uma letra minúscula
        if (!senha.Any(char.IsLower))
            return false;

        // Pelo menos um dígito
        if (!senha.Any(char.IsDigit))
            return false;

        // Pelo menos um caractere especial
        if (!senha.Any(c => "!@#$%^&*()_+-=[]{}|;:',.<>?".Contains(c)))
            return false;

        return true;
    }

    public static string ObterMensagemErro(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            return "A senha não pode estar vazia.";

        var erros = new List<string>();

        if (senha.Length < 8)
            erros.Add("ter no mínimo 8 caracteres");

        if (!senha.Any(char.IsUpper))
            erros.Add("conter pelo menos uma letra MAIÚSCULA");

        if (!senha.Any(char.IsLower))
            erros.Add("conter pelo menos uma letra minúscula");

        if (!senha.Any(char.IsDigit))
            erros.Add("conter pelo menos um número");

        if (!senha.Any(c => "!@#$%^&*()_+-=[]{}|;:',.<>?".Contains(c)))
            erros.Add("conter pelo menos um caractere especial (!@#$%^&* etc.)");

        return erros.Any() 
            ? $"A senha precisa {string.Join(", ", erros)}." 
            : "Senha válida!";
    }

    public static int CalcularForcaSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            return 0;

        int forca = 0;

        // Comprimento
        if (senha.Length >= 8) forca += 20;
        if (senha.Length >= 12) forca += 20;
        if (senha.Length >= 16) forca += 20;

        // Caracteres
        if (senha.Any(char.IsUpper)) forca += 10;
        if (senha.Any(char.IsLower)) forca += 10;
        if (senha.Any(char.IsDigit)) forca += 10;
        if (senha.Any(c => "!@#$%^&*()_+-=[]{}|;:',.<>?".Contains(c))) forca += 10;

        return Math.Min(forca, 100);
    }

    public static string ObterCorForca(int forca)
    {
        return forca switch
        {
            < 40 => "danger",
            < 70 => "warning",
            _ => "success"
        };
    }

    public static string ObterTextoForca(int forca)
    {
        return forca switch
        {
            < 40 => "Fraca",
            < 70 => "Média",
            _ => "Forte"
        };
    }
}
