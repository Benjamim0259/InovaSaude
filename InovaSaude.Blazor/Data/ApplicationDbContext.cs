using Microsoft.EntityFrameworkCore;
using InovaSaude.Blazor.Models;

namespace InovaSaude.Blazor.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Core entities
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<PermissaoUsuario> PermissoesUsuario { get; set; }
    public DbSet<UBS> UBS { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Despesa> Despesas { get; set; }
    public DbSet<Anexo> Anexos { get; set; }
    public DbSet<HistoricoDespesa> HistoricoDespesas { get; set; }

    // Audit and security
    public DbSet<LogAuditoria> LogsAuditoria { get; set; }
    public DbSet<TokenRecuperacaoSenha> TokensRecuperacaoSenha { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<EntityVersion> EntityVersions { get; set; }
    public DbSet<SystemEvent> SystemEvents { get; set; }

    // Import/Export
    public DbSet<ImportacaoLote> ImportacaoLotes { get; set; }
    public DbSet<Importacao> Importacoes { get; set; }
    public DbSet<ImportacaoItem> ImportacaoItens { get; set; }
    public DbSet<DataExport> DataExports { get; set; }

    // Webhooks
    public DbSet<Webhook> Webhooks { get; set; }
    public DbSet<WebhookLog> WebhookLogs { get; set; }

    // Workflows
    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<WorkflowStep> WorkflowSteps { get; set; }
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<WorkflowStepInstance> WorkflowStepInstances { get; set; }

    // Integrations
    public DbSet<Integration> Integrations { get; set; }
    public DbSet<IntegrationLog> IntegrationLogs { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<ExternalSync> ExternalSyncs { get; set; }
    public DbSet<ApiEndpoint> ApiEndpoints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure unique constraints
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<UBS>()
            .HasIndex(u => u.Codigo)
            .IsUnique();

        modelBuilder.Entity<Fornecedor>()
            .HasIndex(f => f.Cnpj)
            .IsUnique();

        modelBuilder.Entity<TokenRecuperacaoSenha>()
            .HasIndex(t => t.Token)
            .IsUnique();

        modelBuilder.Entity<PermissaoUsuario>()
            .HasIndex(p => new { p.UsuarioId, p.Permissao })
            .IsUnique();

        // Configure decimal precision
        modelBuilder.Entity<Categoria>()
            .Property(c => c.OrcamentoMensal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Despesa>()
            .Property(d => d.Valor)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PaymentTransaction>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        // Configure relationships
        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.UbsCoordenadas)
            .WithOne(u => u.Coordenador)
            .HasForeignKey(u => u.CoordenadorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.DespesasCriadas)
            .WithOne(d => d.UsuarioCriacao)
            .HasForeignKey(d => d.UsuarioCriacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.DespesasAprovadas)
            .WithOne(d => d.UsuarioAprovacao)
            .HasForeignKey(d => d.UsuarioAprovacaoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.LogsAuditoria)
            .WithOne(l => l.Usuario)
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.TokensRecuperacao)
            .WithOne(t => t.Usuario)
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Permissoes)
            .WithOne(p => p.Usuario)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure table names
        modelBuilder.Entity<Usuario>().ToTable("usuarios");
        modelBuilder.Entity<PermissaoUsuario>().ToTable("permissoes_usuario");
        modelBuilder.Entity<UBS>().ToTable("ubs");
        modelBuilder.Entity<Fornecedor>().ToTable("fornecedores");
        modelBuilder.Entity<Categoria>().ToTable("categorias");
        modelBuilder.Entity<Despesa>().ToTable("despesas");
        modelBuilder.Entity<Anexo>().ToTable("anexos");
        modelBuilder.Entity<HistoricoDespesa>().ToTable("historico_despesas");
        modelBuilder.Entity<LogAuditoria>().ToTable("logs_auditoria");
        modelBuilder.Entity<TokenRecuperacaoSenha>().ToTable("tokens_recuperacao_senha");
        modelBuilder.Entity<ImportacaoLote>().ToTable("importacao_lotes");
        modelBuilder.Entity<Importacao>().ToTable("importacoes");
        modelBuilder.Entity<ImportacaoItem>().ToTable("importacao_itens");
        modelBuilder.Entity<Webhook>().ToTable("webhooks");
        modelBuilder.Entity<WebhookLog>().ToTable("webhook_logs");
        modelBuilder.Entity<Workflow>().ToTable("workflows");
        modelBuilder.Entity<WorkflowStep>().ToTable("workflow_steps");
        modelBuilder.Entity<WorkflowInstance>().ToTable("workflow_instances");
        modelBuilder.Entity<WorkflowStepInstance>().ToTable("workflow_step_instances");
        modelBuilder.Entity<AuditLog>().ToTable("audit_logs");
        modelBuilder.Entity<EntityVersion>().ToTable("entity_versions");
        modelBuilder.Entity<SystemEvent>().ToTable("system_events");
        modelBuilder.Entity<DataExport>().ToTable("data_exports");
        modelBuilder.Entity<Integration>().ToTable("integrations");
        modelBuilder.Entity<IntegrationLog>().ToTable("integration_logs");
        modelBuilder.Entity<PaymentTransaction>().ToTable("payment_transactions");
        modelBuilder.Entity<ExternalSync>().ToTable("external_syncs");
        modelBuilder.Entity<ApiEndpoint>().ToTable("api_endpoints");

        // Configure indexes for performance
        modelBuilder.Entity<Despesa>()
            .HasIndex(d => d.UbsId);

        modelBuilder.Entity<Despesa>()
            .HasIndex(d => d.CategoriaId);

        modelBuilder.Entity<Despesa>()
            .HasIndex(d => d.FornecedorId);

        modelBuilder.Entity<Despesa>()
            .HasIndex(d => d.Status);

        modelBuilder.Entity<Despesa>()
            .HasIndex(d => d.DataVencimento);

        modelBuilder.Entity<Anexo>()
            .HasIndex(a => a.DespesaId);

        modelBuilder.Entity<HistoricoDespesa>()
            .HasIndex(h => h.DespesaId);

        modelBuilder.Entity<LogAuditoria>()
            .HasIndex(l => l.UsuarioId);

        modelBuilder.Entity<LogAuditoria>()
            .HasIndex(l => l.Entidade);

        modelBuilder.Entity<LogAuditoria>()
            .HasIndex(l => l.CreatedAt);

        modelBuilder.Entity<TokenRecuperacaoSenha>()
            .HasIndex(t => t.UsuarioId);

        modelBuilder.Entity<TokenRecuperacaoSenha>()
            .HasIndex(t => t.Token);

        modelBuilder.Entity<WebhookLog>()
            .HasIndex(w => w.WebhookId);

        modelBuilder.Entity<WebhookLog>()
            .HasIndex(w => new { w.Event, w.CreatedAt });

        modelBuilder.Entity<WorkflowStep>()
            .HasIndex(w => w.WorkflowId);

        modelBuilder.Entity<WorkflowInstance>()
            .HasIndex(w => w.WorkflowId);

        modelBuilder.Entity<WorkflowInstance>()
            .HasIndex(w => new { w.EntityType, w.EntityId });

        modelBuilder.Entity<WorkflowStepInstance>()
            .HasIndex(w => w.WorkflowInstanceId);

        modelBuilder.Entity<WorkflowStepInstance>()
            .HasIndex(w => w.AssignedTo);

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => new { a.EntityType, a.EntityId });

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => new { a.UserId, a.CreatedAt });

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => new { a.Action, a.CreatedAt });

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.CreatedAt);

        modelBuilder.Entity<SystemEvent>()
            .HasIndex(s => new { s.EventType, s.CreatedAt });

        modelBuilder.Entity<IntegrationLog>()
            .HasIndex(i => i.IntegrationId);

        modelBuilder.Entity<PaymentTransaction>()
            .HasIndex(p => p.IntegrationId);

        modelBuilder.Entity<PaymentTransaction>()
            .HasIndex(p => p.TransactionId);

        modelBuilder.Entity<ExternalSync>()
            .HasIndex(e => e.IntegrationId);

        modelBuilder.Entity<ApiEndpoint>()
            .HasIndex(a => a.IntegrationId);
    }
}