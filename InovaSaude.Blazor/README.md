# InovaSaúde - Sistema Blazor Server com C#

Sistema de gestão financeira para Unidades Básicas de Saúde (UBS) desenvolvido em **Blazor Server** com **.NET 8** e **Entity Framework Core**.

## ✅ Status: **FUNCIONANDO**

A aplicação está configurada, compilando e rodando corretamente!

## 🚀 Como Executar

### Pré-requisitos
- .NET 8 SDK instalado
- SQL Server (LocalDB ou Express)

### Executar o Projeto

```bash
# Navegar até a pasta do projeto
cd InovaSaude.Blazor

# Compilar o projeto
dotnet build

# Executar a aplicação
dotnet run
```

A aplicação estará disponível em: **http://localhost:5163**

## 🔐 Login Padrão

Após o primeiro startup, o sistema cria automaticamente um usuário administrador:

- **Email:** admin@inovasaude.com.br
- **Senha:** Admin@123

## 📋 Funcionalidades Implementadas

### Core
- ✅ Autenticação e Autorização com cookies
- ✅ Sistema de permissões por usuário
- ✅ Dashboard com estatísticas em tempo real
- ✅ Gestão de Usuários
- ✅ Gestão de UBS (Unidades Básicas de Saúde)
- ✅ Gestão de Fornecedores
- ✅ Gestão de Categorias de Despesas

### Despesas
- ✅ Cadastro e gestão de despesas
- ✅ Workflow de aprovação
- ✅ Anexos de documentos
- ✅ Histórico de alterações
- ✅ Filtros avançados

### Relatórios
- ✅ Relatório de despesas por período
- ✅ Relatório por UBS
- ✅ Relatório por categoria
- ✅ Exportação de dados

### Auditoria e Logs
- ✅ Log de auditoria de todas as operações
- ✅ Rastreamento de alterações
- ✅ Versionamento de entidades

### Importação e Exportação
- ✅ Importação em lote de despesas
- ✅ Exportação de relatórios
- ✅ Validação de dados importados

### Integrações
- ✅ Sistema de Webhooks
- ✅ Integração com APIs externas
- ✅ Sistema de pagamentos
- ✅ Sincronização com sistemas externos

### Workflows
- ✅ Criação de workflows personalizados
- ✅ Gestão de etapas e aprovações
- ✅ Atribuição de tarefas

## 🏗️ Estrutura do Projeto

```
InovaSaude.Blazor/
├── Components/          # Componentes Blazor reutilizáveis
│   ├── Pages/          # Páginas específicas (Dashboard, Despesas, etc)
│   └── Shared/         # Layout e componentes compartilhados
├── Controllers/        # Controllers MVC (AccountController)
├── Data/              # DbContext e configurações EF Core
├── Models/            # Entidades do domínio
├── Services/          # Serviços de negócio
├── Migrations/        # Migrações do Entity Framework
├── Pages/             # Páginas Razor principais
└── wwwroot/           # Arquivos estáticos (CSS, JS, imagens)
```

## 🛠️ Tecnologias Utilizadas

- **Framework:** .NET 8
- **UI:** Blazor Server
- **ORM:** Entity Framework Core 8
- **Banco de Dados:** SQL Server
- **Autenticação:** Cookie Authentication
- **Hashing de Senhas:** BCrypt.Net
- **CSS:** Bootstrap 5

## 📦 Pacotes NuGet

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.10" />
```

## 🗄️ Banco de Dados

### Connection String

Configurada em `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InovaSaude;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### Migrations

O projeto já possui migrations configuradas. Para aplicar:

```bash
# Aplicar migrations
dotnet ef database update

# Criar nova migration (se necessário)
dotnet ef migrations add NomeDaMigracao
```

## 📊 Seed de Dados

O sistema automaticamente popula dados iniciais no primeiro startup:

- **Usuário Admin** (admin@inovasaude.com.br)
- **Categorias padrão** (Medicamentos, Equipamentos, etc)
- **UBS exemplo** (UBS Centro, UBS Norte, UBS Sul)

## 🎨 Páginas Disponíveis

- `/` - Página inicial
- `/login` - Tela de login
- `/dashboard` - Dashboard principal (requer autenticação)
- `/despesas` - Gestão de despesas
- `/ubs` - Gestão de UBS
- `/relatorios` - Relatórios e analytics

## 🔧 Configuração de Desenvolvimento

### Visual Studio 2022
1. Abrir `InovaSaude.Blazor.csproj`
2. Pressionar F5 para executar

### VS Code
1. Abrir a pasta `InovaSaude.Blazor`
2. Pressionar F5 ou usar o terminal

### Rider
1. Abrir `InovaSaude.Blazor.csproj`
2. Executar configuração de Debug

## 🐛 Resolução de Problemas

### Erro de Conexão com Banco de Dados

Se houver erro de conexão, verifique:

1. SQL Server está rodando
2. Connection string está correta
3. Permissões de acesso ao banco

Para usar SQL Server Express:
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=InovaSaude;Trusted_Connection=True;TrustServerCertificate=True"
```

### Avisos de Compilação

Os avisos sobre nullability e async/await são normais e não afetam o funcionamento. Para corrigi-los, é possível:

- Adicionar verificações de null
- Adicionar operadores await onde apropriado
- Usar nullable reference types adequadamente

## 📈 Próximos Passos (Opcionais)

- [ ] Implementar notificações em tempo real (SignalR)
- [ ] Adicionar gráficos interativos (Chart.js)
- [ ] Implementar cache de dados (Redis)
- [ ] Adicionar testes unitários
- [ ] Containerizar com Docker
- [ ] Deploy em Azure ou AWS

## 📝 Licença

Este projeto é de código privado para uso interno.

## 👥 Suporte

Para questões ou problemas, entre em contato com a equipe de desenvolvimento.

---

**Desenvolvido com ❤️ usando Blazor Server e .NET 8**
