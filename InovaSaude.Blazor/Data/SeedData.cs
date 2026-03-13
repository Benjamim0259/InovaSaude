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

        // Verificar/criar coluna MesReferencia na tabela despesas (workaround PostgreSQL)
        try
        {
            logger.LogInformation("Verificando coluna MesReferencia na tabela despesas...");
            await context.Database.ExecuteSqlRawAsync(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'despesas' AND column_name = 'MesReferencia'
                    ) THEN
                        ALTER TABLE despesas ADD COLUMN ""MesReferencia"" timestamp with time zone NOT NULL DEFAULT NOW();
                        CREATE INDEX IF NOT EXISTS ""IX_despesas_MesReferencia"" ON despesas (""MesReferencia"");
                    END IF;
                END $$;
            ");

            // Remover colunas antigas se existirem
            await context.Database.ExecuteSqlRawAsync(@"
                DO $$ 
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'despesas' AND column_name = 'DataVencimento') THEN
                        ALTER TABLE despesas DROP COLUMN ""DataVencimento"";
                    END IF;
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'despesas' AND column_name = 'DataPagamento') THEN
                        ALTER TABLE despesas DROP COLUMN ""DataPagamento"";
                    END IF;
                END $$;
            ");
            logger.LogInformation("Coluna MesReferencia verificada/criada com sucesso!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao verificar coluna MesReferencia");
        }

        // Criar tabela funcionarios se nao existir (workaround para migrations)
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

        // Atualizar categoria Medicamentos para Farmacia (se existir)
        try
        {
            logger.LogInformation("Atualizando categoria Medicamentos para Farmacia...");
            await context.Database.ExecuteSqlRawAsync(@"
                UPDATE categorias 
                SET ""Nome"" = 'Farmacia', 
                    ""Descricao"" = 'Medicamentos, vacinas e insumos farmaceuticos',
                    ""Icone"" = 'oi-heart'
                WHERE ""Nome"" = 'Medicamentos';
            ");
            logger.LogInformation("Categoria atualizada com sucesso!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao atualizar categoria (pode nao existir)");
        }

        // Seed usuarios de teste (um por perfil)
        var usuariosTeste = new[]
        {
            new { Email = "admin@inovasaude.com.br",        Senha = "Admin@123",  Nome = "Administrador",     Perfil = PerfilUsuario.ADMIN },
            new { Email = "coordenador@inovasaude.com.br",  Senha = "Coord@123",  Nome = "Coordenador Teste", Perfil = PerfilUsuario.COORDENADOR },
            new { Email = "gestor@inovasaude.com.br",       Senha = "Gestor@123", Nome = "Gestor Teste",      Perfil = PerfilUsuario.GESTOR },
            new { Email = "auditor@inovasaude.com.br",      Senha = "Audit@123",  Nome = "Auditor Teste",     Perfil = PerfilUsuario.AUDITOR },
            new { Email = "operador@inovasaude.com.br",     Senha = "Oper@123",   Nome = "Operador Teste",    Perfil = PerfilUsuario.OPERADOR },
            new { Email = "visualizador@inovasaude.com.br", Senha = "Visual@123", Nome = "Visualizador Teste",Perfil = PerfilUsuario.VISUALIZADOR },
        };

        foreach (var u in usuariosTeste)
        {
            if (!await context.Usuarios.AnyAsync(x => x.Email == u.Email))
            {
                context.Usuarios.Add(new Usuario
                {
                    Nome = u.Nome,
                    Email = u.Email,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(u.Senha),
                    Perfil = u.Perfil,
                    Status = "ATIVO",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
        await context.SaveChangesAsync();

        // Seed default categories
        if (!await context.Categorias.AnyAsync())
        {
            var categorias = new List<Categoria>
            {
                new Categoria
                {
                    Nome = "Farmacia",
                    Tipo = "DESPESA",
                    Descricao = "Medicamentos, vacinas e insumos farmaceuticos",
                    OrcamentoMensal = 35000,
                    Cor = "#dc3545",
                    Icone = "oi-heart",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Material Medico",
                    Tipo = "DESPESA",
                    Descricao = "Equipamentos medicos, luvas, seringas, etc",
                    OrcamentoMensal = 20000,
                    Cor = "#0d6efd",
                    Icone = "oi-medical-cross",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Contas Fixas",
                    Tipo = "DESPESA",
                    Descricao = "Agua, luz, telefone, internet",
                    OrcamentoMensal = 8000,
                    Cor = "#ffc107",
                    Icone = "oi-bolt",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Pessoal",
                    Tipo = "DESPESA",
                    Descricao = "Salarios, encargos e beneficios",
                    OrcamentoMensal = 120000,
                    Cor = "#198754",
                    Icone = "oi-people",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Infraestrutura",
                    Tipo = "DESPESA",
                    Descricao = "Manutencao predial, reformas e melhorias",
                    OrcamentoMensal = 25000,
                    Cor = "#6c757d",
                    Icone = "oi-wrench",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Servicos Terceirizados",
                    Tipo = "DESPESA",
                    Descricao = "Limpeza, seguranca, jardinagem, ambulancia",
                    OrcamentoMensal = 18000,
                    Cor = "#0dcaf0",
                    Icone = "oi-briefcase",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Material de Expediente",
                    Tipo = "DESPESA",
                    Descricao = "Papelaria, impressao, materiais de escritorio",
                    OrcamentoMensal = 3000,
                    Cor = "#212529",
                    Icone = "oi-clipboard",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Categoria
                {
                    Nome = "Alimentacao",
                    Tipo = "DESPESA",
                    Descricao = "Cozinha, refeitorio e alimentacao de pacientes",
                    OrcamentoMensal = 12000,
                    Cor = "#fd7e14",
                    Icone = "oi-basket",
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
                Codigo = "2000001",
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
                Codigo = "2000002",
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
