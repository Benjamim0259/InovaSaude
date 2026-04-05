# InovaSaúde - Frontend Blazor Server

Este é o frontend do sistema InovaSaúde, desenvolvido com **Blazor Server** e **MudBlazor**.

## 🚀 Tecnologias

- **.NET 8.0**
- **Blazor Server**
- **MudBlazor 7.0** - Biblioteca de componentes UI
- **Blazored.LocalStorage** - Armazenamento local
- **ClosedXML** - Exportação para Excel
- **CsvHelper** - Exportação para CSV
- **System.IdentityModel.Tokens.Jwt** - Autenticação JWT

## 📋 Funcionalidades

### Autenticação
- Login com JWT
- Armazenamento de token no LocalStorage
- Proteção de rotas com `[Authorize]`
- Estado de autenticação persistente

### Dashboard
- Visualização de métricas principais
- Total de despesas
- Quantidade de UBS
- Despesas pendentes

### Gestão de UBS
- Listagem de UBS
- Busca por nome
- Exportação para Excel/CSV
- Criação e edição (Admin/Gestor)
- Exclusão (Admin)

### Gestão de Despesas
- Listagem de despesas
- Filtros por data e busca
- Exportação para Excel/CSV
- Status visual com cores
- Visualização, criação e edição

## 🔧 Configuração

### Requisitos
- .NET 8.0 SDK
- Backend da API rodando

### Configuração da API

Edite o arquivo `appsettings.json`:

```json
{
  "ApiBaseUrl": "http://localhost:5000"
}
```

## ▶️ Execução

### Modo Desenvolvimento

```bash
cd InovaSaude.Web
dotnet run
```

A aplicação estará disponível em:
- http://localhost:5141

### Build de Produção

```bash
dotnet publish -c Release -o ./publish
```

## 📁 Estrutura do Projeto

```
InovaSaude.Web/
├── Auth/                       # Autenticação customizada
│   └── CustomAuthStateProvider.cs
├── Components/
│   ├── Layout/                # Layouts
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Pages/                 # Páginas
│   │   ├── Login.razor
│   │   ├── Dashboard.razor
│   │   ├── UBSList.razor
│   │   └── DespesasList.razor
│   ├── App.razor             # Root da aplicação
│   └── Routes.razor          # Configuração de rotas
├── Models/                    # DTOs
│   ├── AuthDtos.cs
│   ├── UbsDtos.cs
│   ├── DespesaDtos.cs
│   ├── DashboardDtos.cs
│   └── Enums.cs
├── Services/                  # Serviços
│   ├── IApiService.cs        # Interface da API
│   ├── ApiService.cs         # Implementação da API
│   ├── IExportService.cs     # Interface de exportação
│   └── ExportService.cs      # Exportação Excel/CSV
├── wwwroot/
│   ├── js/
│   │   └── fileDownload.js   # Download de arquivos
│   └── css/
└── Program.cs                 # Configuração da aplicação
```

## 🔐 Autenticação

O sistema utiliza JWT (JSON Web Token) para autenticação:

1. O usuário faz login através da página `/login`
2. O token JWT é armazenado no LocalStorage
3. Todas as requisições HTTP incluem o token no header `Authorization`
4. Rotas protegidas redirecionam para login se não autenticado

## 📊 Exportação de Dados

As listagens de UBS e Despesas possuem botões para exportação:

- **Excel (.xlsx)**: Usando ClosedXML
- **CSV**: Usando CsvHelper

Os arquivos são gerados no servidor e baixados via JavaScript.

## 🎨 UI/UX

- Interface responsiva com MudBlazor
- Tema Material Design
- Ícones Material Icons
- Componentes consistentes (Cards, Tables, Forms, etc.)
- Feedback visual com Snackbars
- Estados de loading

## 📝 Notas

- Este projeto substitui o frontend React anterior
- Mantém compatibilidade total com a API backend existente
- Todas as funcionalidades do React foram migradas
- Adiciona exportação de dados (nova funcionalidade)
