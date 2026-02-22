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

        // Criar tabela funcionarios se não existir (workaround para migrations)
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS funcionarios (
                    ""Id"" text NOT NULL,
                    ""Nome"" character varying(255) NOT NULL,
                    ""Salario"" numeric(18,2) NOT NULL,
                    ""UbsId"" text NOT NULL,
                    ""Cargo"" character varying(50),
                    ""CreatedAt"" timestamp with time zone NOT NULL,
                    ""UpdatedAt"" timestamp with time zone NOT NULL,
                    CONSTRAINT ""PK_funcionarios"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_funcionarios_ubs_UbsId"" FOREIGN KEY (""UbsId"") REFERENCES ubs(""Id"") ON DELETE CASCADE
                );
                CREATE INDEX IF NOT EXISTS ""IX_funcionarios_UbsId"" ON funcionarios (""UbsId"");
            ");
        }
        catch (Exception ex)
        {
            var logger = provider.GetRequiredService<ILogger<Program>>();
            logger.LogWarning(ex, "Tabela funcionarios já existe ou erro ao criar");
        }

        // Seed Admin user
        if (!await context.Usuarios.AnyAsync(u => u.Perfil == PerfilUsuario.ADMIN))
        {
            var admin = new Usuario
            {
                Nome = "Administrador",
                Email = "admin@inovasaude.com.br",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Perfil = PerfilUsuario.ADMIN,
                Status = "ATIVO",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();
        }

        // Seed additional test users
        if (await context.Usuarios.CountAsync() == 1)
        {
            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Nome = "Coordenador Teste",
                    Email = "coordenador@inovasaude.com.br",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Coord@123"),
                    Perfil = PerfilUsuario.COORDENADOR,
                    Status = "ATIVO",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Usuario
                {
                    Nome = "Gestor Teste",
                    Email = "gestor@inovasaude.com.br",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Gestor@123"),
                    Perfil = PerfilUsuario.GESTOR,
                    Status = "ATIVO",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Usuario
                {
                    Nome = "Operador Teste",
                    Email = "operador@inovasaude.com.br",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Oper@123"),
                    Perfil = PerfilUsuario.OPERADOR,
                    Status = "ATIVO",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Usuarios.AddRange(usuarios);
            await context.SaveChangesAsync();
        }

        // Seed default categories
        if (!await context.Categorias.AnyAsync())
        {
            var categorias = new List<Categoria>
            {
                new Categoria
                {
                    Nome = "Medicamentos",
                    Tipo = "DESPESA",
                    Descricao = "Medicamentos, vacinas e insumos farmacêuticos",
                    OrcamentoMensal = 35000,
                    Cor = "#dc3545",
                    Icone = "💊",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Material Médico",
                    Tipo = "DESPESA",
                    Descricao = "Equipamentos médicos, luvas, seringas, etc",
                    OrcamentoMensal = 20000,
                    Cor = "#0d6efd",
                    Icone = "🩺",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Contas Fixas",
                    Tipo = "DESPESA",
                    Descricao = "Água, luz, telefone, internet",
                    OrcamentoMensal = 8000,
                    Cor = "#ffc107",
                    Icone = "💡",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Pessoal",
                    Tipo = "DESPESA",
                    Descricao = "Salários, encargos e benefícios",
                    OrcamentoMensal = 120000,
                    Cor = "#198754",
                    Icone = "👥",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Infraestrutura",
                    Tipo = "DESPESA",
                    Descricao = "Manutenção predial, reformas e melhorias",
                    OrcamentoMensal = 25000,
                    Cor = "#6c757d",
                    Icone = "🏗️",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Serviços Terceirizados",
                    Tipo = "DESPESA",
                    Descricao = "Limpeza, segurança, jardinagem, ambulância",
                    OrcamentoMensal = 18000,
                    Cor = "#0dcaf0",
                    Icone = "🚑",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Material de Expediente",
                    Tipo = "DESPESA",
                    Descricao = "Papelaria, impressão, materiais de escritório",
                    OrcamentoMensal = 3000,
                    Cor = "#212529",
                    Icone = "📚",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Alimentação",
                    Tipo = "DESPESA",
                    Descricao = "Cozinha, refeitório e alimentação de pacientes",
                    OrcamentoMensal = 12000,
                    Cor = "#fd7e14",
                    Icone = "🍽️",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
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
