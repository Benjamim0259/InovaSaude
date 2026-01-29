using BCrypt.Net;
using InovaSaude.Blazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InovaSaude.Blazor.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<ApplicationDbContext>();

        // Apply pending migrations
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            await context.Database.MigrateAsync();
        }

        // Seed Admin user
        if (!await context.Usuarios.AnyAsync(u => u.Perfil == PerfilUsuario.ADMIN))
        {
            var admin = new Usuario
            {
                Nome = "Administrador",
                Email = "admin@inovasaude.local",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Perfil = PerfilUsuario.ADMIN,
                Status = "ATIVO",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();
        }

        // Seed default categories
        if (!await context.Categorias.AnyAsync())
        {
            var categorias = new List<Categoria>
            {
                new Categoria { Nome = "Medicamentos", Tipo = "DESPESA", Descricao = "Gastos com medicamentos", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Categoria { Nome = "Serviços", Tipo = "DESPESA", Descricao = "Serviços terceirizados", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Categoria { Nome = "Infraestrutura", Tipo = "DESPESA", Descricao = "Melhorias e manutenção", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Categorias.AddRange(categorias);
            await context.SaveChangesAsync();
        }

        // Seed a sample UBS
        if (!await context.UBS.AnyAsync())
        {
            var ubs = new UBS
            {
                Nome = "UBS Central",
                Codigo = "UBS001",
                Endereco = "Rua Principal, 123",
                Bairro = "Centro",
                Cep = "00000-000",
                Status = "ATIVA",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.UBS.Add(ubs);
            await context.SaveChangesAsync();
        }
    }
}
