using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BCrypt.Net;

namespace InovaSaude.Blazor.Services;

public class ImportacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ImportacaoService> _logger;

    public ImportacaoService(ApplicationDbContext context, ILogger<ImportacaoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Importacao> CreateImportacaoAsync(
        string nomeArquivo,
        string tipo,
        string? descricao = null,
        string? criadoPor = null)
    {
        var importacao = new Importacao
        {
            NomeArquivo = nomeArquivo,
            Tipo = tipo,
            Status = "PENDING",
            Descricao = descricao,
            CriadoPor = criadoPor,
            CriadoEm = DateTime.UtcNow
        };

        _context.Importacoes.Add(importacao);
        await _context.SaveChangesAsync();

        return importacao;
    }

    public async Task UpdateImportacaoStatusAsync(string importacaoId, string status, string? erro = null)
    {
        var importacao = await _context.Importacoes.FindAsync(importacaoId);
        if (importacao == null) return;

        importacao.Status = status;
        if (!string.IsNullOrEmpty(erro))
            importacao.Erro = erro;

        importacao.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<List<Importacao>> GetImportacoesAsync(
        string? tipo = null,
        string? status = null,
        string? criadoPor = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null)
    {
        var query = _context.Importacoes.AsQueryable();

        if (!string.IsNullOrEmpty(tipo))
            query = query.Where(i => i.Tipo == tipo);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(i => i.Status == status);

        if (!string.IsNullOrEmpty(criadoPor))
            query = query.Where(i => i.CriadoPor == criadoPor);

        if (dataInicio.HasValue)
            query = query.Where(i => i.CriadoEm >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(i => i.CriadoEm <= dataFim.Value);

        return await query
            .OrderByDescending(i => i.CriadoEm)
            .ToListAsync();
    }

    public async Task<Importacao?> GetImportacaoByIdAsync(string id)
    {
        return await _context.Importacoes
            .Include(i => i.Itens)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task ProcessarImportacaoDespesasAsync(string importacaoId, string csvContent, string criadoPor)
    {
        var importacao = await _context.Importacoes.FindAsync(importacaoId);
        if (importacao == null) throw new InvalidOperationException("Importação não encontrada");

        importacao.Status = "PROCESSING";
        await _context.SaveChangesAsync();

        try
        {
            var lines = csvContent.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            if (lines.Length < 2) throw new InvalidOperationException("Arquivo CSV deve ter pelo menos um cabeçalho e uma linha de dados");

            var header = lines[0].Split(',').Select(h => h.Trim().Trim('"')).ToArray();
            var dataLines = lines.Skip(1).ToArray();

            var processedCount = 0;
            var errorCount = 0;

            foreach (var line in dataLines)
            {
                try
                {
                    var values = ParseCsvLine(line);
                    var despesa = await CreateDespesaFromCsvAsync(values, header, criadoPor);

                    var item = new ImportacaoItem
                    {
                        ImportacaoId = importacaoId,
                        DadosOriginais = line,
                        Status = "SUCCESS",
                        EntidadeCriadaId = despesa.Id,
                        EntidadeTipo = "Despesa"
                    };

                    _context.ImportacaoItens.Add(item);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    var item = new ImportacaoItem
                    {
                        ImportacaoId = importacaoId,
                        DadosOriginais = line,
                        Status = "ERROR",
                        Erro = ex.Message
                    };

                    _context.ImportacaoItens.Add(item);
                    errorCount++;
                }
            }

            importacao.Status = "COMPLETED";
            importacao.TotalRegistros = dataLines.Length;
            importacao.RegistrosProcessados = processedCount;
            importacao.RegistrosErro = errorCount;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            importacao.Status = "FAILED";
            importacao.Erro = ex.Message;
            await _context.SaveChangesAsync();
            throw;
        }
    }

    private string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = "";
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current += '"';
                    i++; // Skip next quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.Trim().Trim('"'));
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current.Trim().Trim('"'));
        return result.ToArray();
    }

    private async Task<Despesa> CreateDespesaFromCsvAsync(string[] values, string[] header, string criadoPor)
    {
        var despesa = new Despesa
        {
            UsuarioCriacaoId = criadoPor,
            CreatedAt = DateTime.UtcNow
        };

        for (int i = 0; i < header.Length && i < values.Length; i++)
        {
            var field = header[i].ToLower();
            var value = values[i];

            switch (field)
            {
                case "data":
                case "data_despesa":
                    despesa.DataVencimento = DateTime.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "valor":
                case "valor_total":
                    despesa.Valor = decimal.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "categoria":
                    despesa.CategoriaId = await GetOrCreateCategoriaAsync(value);
                    break;
                case "ubs_id":
                case "ubs":
                    despesa.EsfId = value;
                    break;
                case "fornecedor":
                    despesa.FornecedorId = await GetOrCreateFornecedorAsync(value);
                    break;
                case "descricao":
                case "observacao":
                    despesa.Descricao = value;
                    break;
                case "numero_nota":
                case "nota_fiscal":
                    despesa.NumeroNota = value;
                    break;
                case "tipo":
                    despesa.Tipo = value;
                    break;
                case "status":
                    // Status removido;
                    break;
            }
        }

        // Validate required fields
        if (despesa.Valor <= 0) throw new InvalidOperationException("Valor da despesa deve ser maior que zero");
        if (string.IsNullOrEmpty(despesa.CategoriaId)) throw new InvalidOperationException("Categoria é obrigatória");
        if (string.IsNullOrEmpty(despesa.EsfId)) throw new InvalidOperationException("UBS é obrigatória");
        if (string.IsNullOrEmpty(despesa.Descricao)) throw new InvalidOperationException("Descrição é obrigatória");
        if (string.IsNullOrEmpty(despesa.Tipo)) throw new InvalidOperationException("Tipo é obrigatório");

        _context.Despesas.Add(despesa);
        await _context.SaveChangesAsync();

        return despesa;
    }

    private async Task<string> GetOrCreateCategoriaAsync(string categoriaNome)
    {
        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Nome.ToLower() == categoriaNome.ToLower());

        if (categoria == null)
        {
            categoria = new Categoria
            {
                Nome = categoriaNome,
                Descricao = $"Categoria criada automaticamente durante importação",
                CreatedAt = DateTime.UtcNow
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
        }

        return categoria.Id;
    }

    private async Task<string?> GetOrCreateFornecedorAsync(string fornecedorNome)
    {
        if (string.IsNullOrWhiteSpace(fornecedorNome)) return null;

        var fornecedor = await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.RazaoSocial.ToLower() == fornecedorNome.ToLower());

        if (fornecedor == null)
        {
            fornecedor = new Fornecedor
            {
                RazaoSocial = fornecedorNome,
                CreatedAt = DateTime.UtcNow
            };

            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();
        }

        return fornecedor.Id;
    }

    public async Task ProcessarImportacaoUsuariosAsync(string importacaoId, string csvContent, string criadoPor)
    {
        var importacao = await _context.Importacoes.FindAsync(importacaoId);
        if (importacao == null) throw new InvalidOperationException("Importação não encontrada");

        importacao.Status = "PROCESSING";
        await _context.SaveChangesAsync();

        try
        {
            var lines = csvContent.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            if (lines.Length < 2) throw new InvalidOperationException("Arquivo CSV deve ter pelo menos um cabeçalho e uma linha de dados");

            var header = lines[0].Split(',').Select(h => h.Trim().Trim('"')).ToArray();
            var dataLines = lines.Skip(1).ToArray();

            var processedCount = 0;
            var errorCount = 0;

            foreach (var line in dataLines)
            {
                try
                {
                    var values = ParseCsvLine(line);
                    var usuario = await CreateUsuarioFromCsvAsync(values, header, criadoPor);

                    var item = new ImportacaoItem
                    {
                        ImportacaoId = importacaoId,
                        DadosOriginais = line,
                        Status = "SUCCESS",
                        EntidadeCriadaId = usuario.Id,
                        EntidadeTipo = "Usuario"
                    };

                    _context.ImportacaoItens.Add(item);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    var item = new ImportacaoItem
                    {
                        ImportacaoId = importacaoId,
                        DadosOriginais = line,
                        Status = "ERROR",
                        Erro = ex.Message
                    };

                    _context.ImportacaoItens.Add(item);
                    errorCount++;
                }
            }

            importacao.Status = "COMPLETED";
            importacao.TotalRegistros = dataLines.Length;
            importacao.RegistrosProcessados = processedCount;
            importacao.RegistrosErro = errorCount;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            importacao.Status = "FAILED";
            importacao.Erro = ex.Message;
            await _context.SaveChangesAsync();
            throw;
        }
    }

    private async Task<Usuario> CreateUsuarioFromCsvAsync(string[] values, string[] header, string criadoPor)
    {
        var usuario = new Usuario
        {
            CreatedAt = DateTime.UtcNow
        };

        for (int i = 0; i < header.Length && i < values.Length; i++)
        {
            var field = header[i].ToLower();
            var value = values[i];

            switch (field)
            {
                case "nome":
                    usuario.Nome = value;
                    break;
                case "email":
                    usuario.Email = value;
                    break;
                case "senha":
                case "password":
                    usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(value);
                    break;
                case "perfil":
                case "role":
                    usuario.Perfil = Enum.Parse<PerfilUsuario>(value, true);
                    break;
                case "ubs_id":
                    usuario.EsfId = value;
                    break;
                case "ativo":
                case "active":
                    usuario.Status = bool.Parse(value) ? "ATIVO" : "INATIVO";
                    break;
                case "telefone":
                case "phone":
                    usuario.Telefone = value;
                    break;
            }
        }

        // Validate required fields
        if (string.IsNullOrEmpty(usuario.Nome)) throw new InvalidOperationException("Nome é obrigatório");
        if (string.IsNullOrEmpty(usuario.Email)) throw new InvalidOperationException("Email é obrigatório");
        if (string.IsNullOrEmpty(usuario.SenhaHash)) throw new InvalidOperationException("Senha é obrigatória");

        // Check if email already exists
        var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);
        if (existingUser != null) throw new InvalidOperationException($"Usuário com email {usuario.Email} já existe");

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return usuario;
    }

    public async Task<List<ImportacaoItem>> GetImportacaoItensAsync(string importacaoId, string? status = null)
    {
        var query = _context.ImportacaoItens.Where(i => i.ImportacaoId == importacaoId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(i => i.Status == status);

        return await query.OrderBy(i => i.CriadoEm).ToListAsync();
    }

    public async Task DeleteImportacaoAsync(string importacaoId)
    {
        var importacao = await _context.Importacoes
            .Include(i => i.Itens)
            .FirstOrDefaultAsync(i => i.Id == importacaoId);

        if (importacao != null)
        {
            // Remove associated items
            _context.ImportacaoItens.RemoveRange(importacao.Itens);

            // Remove the import
            _context.Importacoes.Remove(importacao);

            await _context.SaveChangesAsync();
        }
    }
}
