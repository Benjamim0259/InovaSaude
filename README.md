# InovaSaúde

Sistema de gestão para Unidades Básicas de Saúde (UBS) e Estratégias de Saúde da Família (ESF), desenvolvido em **Blazor Server** com **.NET 8**.

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express ou instância completa)

## Como Executar

```bash
cd InovaSaude.Blazor
dotnet build
dotnet run
```

A aplicação estará disponível em: **http://localhost:5163**

## Login Padrão

Ao iniciar pela primeira vez, o sistema cria automaticamente um usuário administrador:

| Campo | Valor                        |
|-------|------------------------------|
| Email | `admin@inovasaude.com.br`    |
| Senha | `Admin@123`                  |

## Funcionalidades

### Gestão Geral
- Autenticação e autorização com cookies
- Sistema de permissões por perfil (Admin, Coordenador, Gestor, Auditor, Operador, Visualizador)
- Dashboard com estatísticas em tempo real

### Cadastros
- Usuários do sistema
- UBS / ESF (Unidades Básicas de Saúde / Estratégias de Saúde da Família)
- Funcionários (com cargos predefinidos ou cargo personalizado)
- Fornecedores
- Categorias de despesas

### Financeiro
- Cadastro e gestão de despesas
- Workflow de aprovação com etapas configuráveis
- Anexos de documentos comprobatórios
- Histórico de alterações
- Relatórios financeiros por período, UBS e categoria
- Exportação de dados

### Estoque e Farmácia
- Controle de estoque de medicamentos
- Pedidos de medicamentos com aprovação

### Auditoria
- Log de auditoria de todas as operações
- Rastreamento de alterações
- Versionamento de entidades

### Integrações
- Sistema de Webhooks
- Integração com APIs externas
- Backup e restauração de dados

## Estrutura do Projeto

```
InovaSaude.Blazor/
├── Controllers/        # Controllers (Account, Backup)
├── Data/               # DbContext e seed de dados
├── Helpers/            # Utilitários
├── Migrations/         # Migrações do Entity Framework
├── Models/             # Entidades e enums do domínio
├── Pages/              # Páginas Blazor
├── Services/           # Serviços de negócio
├── Shared/             # Layout e componentes compartilhados
└── wwwroot/            # Arquivos estáticos (CSS, JS)
```

## Tecnologias

| Tecnologia            | Versão / Detalhe       |
|-----------------------|------------------------|
| .NET                  | 8                      |
| Blazor Server         | —                      |
| Entity Framework Core | 8                      |
| Banco de Dados        | SQL Server             |
| Autenticação          | Cookie Authentication  |
| Hash de Senhas        | BCrypt.Net             |
| CSS                   | Bootstrap 5            |

## Banco de Dados

A connection string está configurada em `InovaSaude.Blazor/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InovaSaude;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

Para aplicar as migrações:

```bash
cd InovaSaude.Blazor
dotnet ef database update
```

O sistema popula dados iniciais automaticamente no primeiro startup (usuário admin, categorias padrão e UBS exemplo).

## Páginas Principais

| Rota                        | Descrição                      |
|-----------------------------|--------------------------------|
| `/login`                    | Tela de login                  |
| `/dashboard`                | Dashboard principal            |
| `/gerenciar-esf`            | Gestão de UBS / ESF            |
| `/gerenciar-funcionarios`   | Gestão de funcionários         |
| `/gerenciar-despesas`       | Gestão de despesas             |
| `/gerenciar-usuarios`       | Gestão de usuários             |
| `/gerenciar-estoque`        | Controle de estoque            |
| `/pedidos-medicamentos`     | Pedidos de medicamentos        |
| `/relatorios-financeiros`   | Relatórios financeiros         |
| `/gerar-relatorios`         | Geração de relatórios          |
| `/integracoes-externas`     | Integrações externas           |
| `/workflows`                | Workflows de aprovação         |
| `/backup`                   | Backup e restauração           |
| `/meu-perfil`               | Perfil do usuário              |

## Build para Produção

```bash
cd InovaSaude.Blazor
dotnet publish -c Release -o ./publish
```

## Licença

Este projeto é de código privado para uso interno.
