using InovaSaude.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InovaSaude.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Municipio> Municipios { get; set; } = null!;
    public DbSet<UBS> UbsList { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<Despesa> Despesas { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configurações de precisão para decimal
        builder.Entity<Despesa>()
            .Property(d => d.Valor)
            .HasPrecision(18, 2);

        // Configurações de relacionamentos
        builder.Entity<Usuario>()
            .HasOne(u => u.Municipio)
            .WithMany(m => m.Usuarios)
            .HasForeignKey(u => u.MunicipioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Usuario>()
            .HasOne(u => u.Ubs)
            .WithMany(ubs => ubs.Coordenadores)
            .HasForeignKey(u => u.UbsId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<UBS>()
            .HasOne(ubs => ubs.Municipio)
            .WithMany(m => m.UbsList)
            .HasForeignKey(ubs => ubs.MunicipioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Despesa>()
            .HasOne(d => d.Ubs)
            .WithMany(ubs => ubs.Despesas)
            .HasForeignKey(d => d.UbsId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Despesa>()
            .HasOne(d => d.Categoria)
            .WithMany(c => c.Despesas)
            .HasForeignKey(d => d.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Despesa>()
            .HasOne(d => d.Usuario)
            .WithMany(u => u.Despesas)
            .HasForeignKey(d => d.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.Entity<Usuario>()
            .HasIndex(u => u.Cpf)
            .IsUnique();

        builder.Entity<UBS>()
            .HasIndex(u => u.Cnes)
            .IsUnique();

        builder.Entity<Municipio>()
            .HasIndex(m => m.CodigoIbge)
            .IsUnique();

        // Filtros de query global para soft delete
        builder.Entity<Municipio>().HasQueryFilter(m => !m.IsDeleted);
        builder.Entity<UBS>().HasQueryFilter(u => !u.IsDeleted);
        builder.Entity<Categoria>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<Despesa>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<Usuario>().HasQueryFilter(u => !u.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Atualizar UpdatedAt automaticamente
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity entity)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Usuario usuario)
            {
                usuario.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
