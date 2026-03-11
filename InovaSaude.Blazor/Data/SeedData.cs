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
        var logger = provider.GetRequiredService<ILogger<Program>>();

        try
        {
            // Apply pending migrations
            logger.LogInformation("Verificando migrations pendentes...");
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Aplicando {pendingMigrations.Count()} migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations aplicadas com sucesso!");
            }
            else
            {
                logger.LogInformation("Nenhuma migration pendente.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao aplicar migrations. Continuando...");
        }

        // Criar tabela funcionarios se não existir (workaround para migrations)
        try
        {
            logger.LogInformation("Verificando tabela funcionarios...");

            // Tentar verificar se a tabela existe consultando
            var tabelaExiste = await context.Database.ExecuteSqlRawAsync(@"
                SELECT 1 FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'funcionarios'
            ");

            logger.LogInformation("Criando tabela funcionarios se não existir...");
            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS funcionarios (
                    ""Id"" text NOT NULL,
                    ""Nome"" character varying(255) NOT NULL,
                    ""Salario"" numeric(18,2) NOT NULL,
                    ""EsfId"" text NOT NULL,
                    ""Cargo"" character varying(50),
                    ""CreatedAt"" timestamp with time zone NOT NULL,
                    ""UpdatedAt"" timestamp with time zone NOT NULL,
                    CONSTRAINT ""PK_funcionarios"" PRIMARY KEY (""Id"")
                );
            ");

            // Adicionar FK separadamente
            await context.Database.ExecuteSqlRawAsync(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_funcionarios_esf_EsfId'
                    ) THEN
                        ALTER TABLE funcionarios 
                        ADD CONSTRAINT ""FK_funcionarios_esf_EsfId"" 
                        FOREIGN KEY (""EsfId"") REFERENCES esf(""Id"") ON DELETE CASCADE;
                    END IF;
                END $$;
            ");

            await context.Database.ExecuteSqlRawAsync(@"
                CREATE INDEX IF NOT EXISTS ""IX_funcionarios_EsfId"" ON funcionarios (""EsfId"");
            ");

            logger.LogInformation("Tabela funcionarios verificada/criada com sucesso!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar tabela funcionarios");
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

        // Seed a sample ESF
        if (!await context.ESF.AnyAsync())
        {
            var esf1 = new ESF
            {
                Nome = "ESF Central",
                Cnes = "2000001",
                Codigo = "ESF001",
                Endereco = "Rua Principal, 123",
                Bairro = "Centro",
                Cep = "00000-000",
                Status = "ATIVA",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var esf2 = new ESF
            {
                Nome = "ESF Vila Nova",
                Cnes = "2000002",
                Codigo = "ESF002",
                Endereco = "Av. Secundária, 456",
                Bairro = "Vila Nova",
                Cep = "11111-111",
                Status = "ATIVA",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.ESF.Add(esf1);
            context.ESF.Add(esf2);
            await context.SaveChangesAsync();
        }
    }
}
