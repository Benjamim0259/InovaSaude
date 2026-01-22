# Guia de Migração: React → Blazor Server

## 📋 Resumo da Migração

Este documento descreve a migração completa do frontend React + TypeScript para Blazor Server + MudBlazor.

## 🔄 Comparação de Tecnologias

### Antes (React)
- **Framework**: React 18 + TypeScript
- **Build Tool**: Vite
- **UI Library**: TailwindCSS
- **Routing**: React Router
- **HTTP Client**: Axios
- **State Management**: React Hooks

### Depois (Blazor Server)
- **Framework**: Blazor Server (.NET 8)
- **Language**: C#
- **UI Library**: MudBlazor 7.0
- **Routing**: Built-in Blazor Router
- **HTTP Client**: HttpClient
- **State Management**: Component State + Services

## ✨ Novas Funcionalidades

1. **Exportação de Dados**
   - Excel (.xlsx) via ClosedXML
   - CSV via CsvHelper
   - Disponível em todas as listagens

2. **Autenticação Melhorada**
   - CustomAuthStateProvider
   - Persistência automática via LocalStorage
   - Refresh automático de token

3. **UI Consistente**
   - Componentes Material Design via MudBlazor
   - Tema unificado
   - Feedback visual melhorado

## 🗂️ Mapeamento de Arquivos

### Autenticação
| React | Blazor |
|-------|--------|
| `src/contexts/AuthContext.tsx` | `Auth/CustomAuthStateProvider.cs` |
| `src/services/auth.ts` | `Services/ApiService.cs` (método LoginAsync) |
| `src/utils/auth.ts` | Built-in no CustomAuthStateProvider |

### Páginas
| React | Blazor |
|-------|--------|
| `src/pages/Login.tsx` | `Components/Pages/Login.razor` |
| `src/pages/Dashboard.tsx` | `Components/Pages/Dashboard.razor` |
| `src/pages/UBS/UBSList.tsx` | `Components/Pages/UBSList.razor` |
| `src/pages/Despesas/DespesasList.tsx` | `Components/Pages/DespesasList.razor` |

### Componentes
| React | Blazor |
|-------|--------|
| `src/components/Layout.tsx` | `Components/Layout/MainLayout.razor` |
| `src/components/Sidebar.tsx` | `Components/Layout/NavMenu.razor` |
| `src/components/PrivateRoute.tsx` | `@attribute [Authorize]` nas páginas |

### Serviços
| React | Blazor |
|-------|--------|
| `src/services/api.ts` | `Services/ApiService.cs` |
| `src/services/ubs.ts` | `Services/ApiService.cs` (métodos UBS) |
| `src/services/despesas.ts` | `Services/ApiService.cs` (métodos Despesas) |
| N/A (nova funcionalidade) | `Services/ExportService.cs` |

### Models/Types
| React | Blazor |
|-------|--------|
| `src/types/auth.ts` | `Models/AuthDtos.cs` |
| `src/types/ubs.ts` | `Models/UbsDtos.cs` |
| `src/types/despesa.ts` | `Models/DespesaDtos.cs` |

## 🔧 Mudanças de Configuração

### Desenvolvimento Local

**React (package.json)**:
```json
{
  "scripts": {
    "dev": "vite",
    "build": "vite build"
  }
}
```

**Blazor**:
```bash
dotnet run
dotnet build
```

### Variáveis de Ambiente

**React (.env)**:
```
VITE_API_URL=http://localhost:5000
```

**Blazor (appsettings.json)**:
```json
{
  "ApiBaseUrl": "http://localhost:5000"
}
```

## 📦 Dependências

### React (package.json)
```json
{
  "react": "^18.0.0",
  "react-router-dom": "^6.0.0",
  "axios": "^1.0.0",
  "tailwindcss": "^3.0.0"
}
```

### Blazor (.csproj)
```xml
<PackageReference Include="MudBlazor" Version="7.0.0" />
<PackageReference Include="Blazored.LocalStorage" Version="4.5.0" />
<PackageReference Include="ClosedXML" Version="0.102.2" />
<PackageReference Include="CsvHelper" Version="31.0.2" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.2" />
```

## 🚀 Deploy

### React
- Build: `npm run build`
- Output: `dist/`
- Servir: Nginx/Apache ou CDN

### Blazor
- Build: `dotnet publish -c Release`
- Output: `bin/Release/net8.0/publish/`
- Servir: Kestrel (self-hosted) ou IIS

## 📊 Benefícios da Migração

1. **Linguagem Única**: C# no frontend e backend
2. **Type Safety**: Compilação estática em vez de TypeScript
3. **Performance**: SignalR para comunicação real-time
4. **Menor Bundle Size**: Sem JavaScript pesado
5. **Integração**: Uso direto dos mesmos DTOs do backend
6. **Produtividade**: Menos contexto switching
7. **Exportação Nativa**: Funcionalidade de exportação integrada

## ⚠️ Considerações

1. **Requer Conexão WebSocket**: Blazor Server usa SignalR
2. **Estado do Servidor**: Cada cliente mantém estado no servidor
3. **Latência**: Interações podem ter latency de rede
4. **Escalabilidade**: Considerar sticky sessions em load balancers

## 🔍 Verificação de Funcionalidades

- [x] Login/Logout
- [x] Dashboard com métricas
- [x] Listagem de UBS
- [x] Filtro e busca de UBS
- [x] Exportação UBS (Excel/CSV)
- [x] Listagem de Despesas
- [x] Filtros de Despesas (data, busca)
- [x] Exportação Despesas (Excel/CSV)
- [x] Proteção de rotas
- [x] Feedback visual (loading, errors, success)
- [ ] CRUD completo de UBS
- [ ] CRUD completo de Despesas
- [ ] Perfil de usuário
- [ ] Gestão de permissões

## 📝 Próximos Passos

1. Completar páginas de CRUD (Create, Edit, Details)
2. Adicionar testes unitários
3. Implementar gestão de perfil
4. Adicionar gráficos no dashboard
5. Otimizar performance
6. Implementar cache
7. Adicionar internacionalização (i18n)
