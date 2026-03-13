using System.Text.Json;
using System.Text.Json.Serialization;
using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Blazor.Services;

public class BackupService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BackupService> _logger;

    public BackupService(ApplicationDbContext context, ILogger<BackupService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BackupData> ExportarAsync()
    {
        _logger.LogInformation("Iniciando exportacao de backup...");

        var backup = new BackupData
        {
            DataExportacao = DateTime.UtcNow,
            Versao = "1.0",
            Usuarios = await _context.Usuarios.AsNoTracking().ToListAsync(),
            ESFs = await _context.ESF.AsNoTracking().ToListAsync(),
            Funcionarios = await _context.Funcionarios.AsNoTracking().ToListAsync(),
            Categorias = await _context.Categorias.AsNoTracking().ToListAsync(),
            Fornecedores = await _context.Fornecedores.AsNoTracking().ToListAsync(),
            Despesas = await _context.Despesas.AsNoTracking().ToListAsync()
        };

        _logger.LogInformation(
            "Backup exportado: {Usuarios} usuarios, {ESFs} ESFs, {Funcionarios} funcionarios, {Categorias} categorias, {Fornecedores} fornecedores, {Despesas} despesas",
            backup.Usuarios.Count, backup.ESFs.Count, backup.Funcionarios.Count,
            backup.Categorias.Count, backup.Fornecedores.Count, backup.Despesas.Count);

        return backup;
    }

    public string SerializarBackup(BackupData backup)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(backup, options);
    }

    public BackupData? DesserializarBackup(string json)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<BackupData>(json, options);
    }

    public async Task<BackupResult> ImportarAsync(BackupData backup)
    {
        var result = new BackupResult();
        _logger.LogInformation("Iniciando importacao de backup...");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Importar Categorias
            foreach (var cat in backup.Categorias)
            {
                if (!await _context.Categorias.AnyAsync(c => c.Id == cat.Id))
                {
                    _context.Categorias.Add(cat);
                    result.CategoriasImportadas++;
                }
                else { result.CategoriasIgnoradas++; }
            }
            await _context.SaveChangesAsync();

            // Importar ESFs
            foreach (var esf in backup.ESFs)
            {
                if (!await _context.ESF.AnyAsync(e => e.Id == esf.Id))
                {
                    // Limpar navigations para evitar conflito
                    esf.Coordenador = null;
                    esf.Usuarios = new List<Usuario>();
                    esf.Funcionarios = new List<Funcionario>();
                    esf.Despesas = new List<Despesa>();
                    _context.ESF.Add(esf);
                    result.ESFsImportadas++;
                }
                else { result.ESFsIgnoradas++; }
            }
            await _context.SaveChangesAsync();

            // Importar Usuarios
            foreach (var user in backup.Usuarios)
            {
                if (!await _context.Usuarios.AnyAsync(u => u.Id == user.Id || u.Email == user.Email))
                {
                    user.Esf = null;
                    user.EsfCoordenadas = new List<ESF>();
                    user.DespesasCriadas = new List<Despesa>();
                    user.LogsAuditoria = new List<LogAuditoria>();
                    user.TokensRecuperacao = new List<TokenRecuperacaoSenha>();
                    user.Permissoes = new List<PermissaoUsuario>();
                    _context.Usuarios.Add(user);
                    result.UsuariosImportados++;
                }
                else { result.UsuariosIgnorados++; }
            }
            await _context.SaveChangesAsync();

            // Importar Fornecedores
            foreach (var forn in backup.Fornecedores)
            {
                if (!await _context.Fornecedores.AnyAsync(f => f.Id == forn.Id))
                {
                    _context.Fornecedores.Add(forn);
                    result.FornecedoresImportados++;
                }
                else { result.FornecedoresIgnorados++; }
            }
            await _context.SaveChangesAsync();

            // Importar Funcionarios
            foreach (var func in backup.Funcionarios)
            {
                if (!await _context.Funcionarios.AnyAsync(f => f.Id == func.Id))
                {
                    func.Esf = null!;
                    _context.Funcionarios.Add(func);
                    result.FuncionariosImportados++;
                }
                else { result.FuncionariosIgnorados++; }
            }
            await _context.SaveChangesAsync();

            // Importar Despesas
            foreach (var desp in backup.Despesas)
            {
                if (!await _context.Despesas.AnyAsync(d => d.Id == desp.Id))
                {
                    desp.Categoria = null!;
                    desp.Esf = null!;
                    desp.Fornecedor = null;
                    desp.UsuarioCriacao = null!;
                    desp.Anexos = new List<Anexo>();
                    desp.HistoricoStatus = new List<HistoricoDespesa>();
                    _context.Despesas.Add(desp);
                    result.DespesasImportadas++;
                }
                else { result.DespesasIgnoradas++; }
            }
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            result.Sucesso = true;

            _logger.LogInformation(
                "Backup importado com sucesso: {U} usuarios, {E} ESFs, {F} funcionarios, {C} categorias, {Fo} fornecedores, {D} despesas",
                result.UsuariosImportados, result.ESFsImportadas, result.FuncionariosImportados,
                result.CategoriasImportadas, result.FornecedoresImportados, result.DespesasImportadas);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            result.Sucesso = false;
            result.Erro = ex.Message;
            _logger.LogError(ex, "Erro ao importar backup");
        }

        return result;
    }
}

public class BackupData
{
    public DateTime DataExportacao { get; set; }
    public string Versao { get; set; } = "1.0";
    public List<Usuario> Usuarios { get; set; } = new();
    public List<ESF> ESFs { get; set; } = new();
    public List<Funcionario> Funcionarios { get; set; } = new();
    public List<Categoria> Categorias { get; set; } = new();
    public List<Fornecedor> Fornecedores { get; set; } = new();
    public List<Despesa> Despesas { get; set; } = new();
}

public class BackupResult
{
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }
    public int UsuariosImportados { get; set; }
    public int UsuariosIgnorados { get; set; }
    public int ESFsImportadas { get; set; }
    public int ESFsIgnoradas { get; set; }
    public int FuncionariosImportados { get; set; }
    public int FuncionariosIgnorados { get; set; }
    public int CategoriasImportadas { get; set; }
    public int CategoriasIgnoradas { get; set; }
    public int FornecedoresImportados { get; set; }
    public int FornecedoresIgnorados { get; set; }
    public int DespesasImportadas { get; set; }
    public int DespesasIgnoradas { get; set; }
}
