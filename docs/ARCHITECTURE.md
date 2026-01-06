# Arquitetura Técnica - InovaSaúde

## Visão Geral

O InovaSaúde é uma aplicação web para gestão financeira de Unidades Básicas de Saúde (UBS), construída com arquitetura moderna e escalável.

## Arquitetura Geral

```
┌─────────────┐     ┌─────────────┐     ┌──────────────┐
│   Frontend  │────▶│   Backend   │────▶│  PostgreSQL  │
│   (React)   │     │   (.NET 8)  │     │   Database   │
└─────────────┘     └─────────────┘     └──────────────┘
       │                    │
       │                    │
       ▼                    ▼
   ┌────────┐         ┌─────────┐
   │ Nginx  │         │  Logs   │
   └────────┘         └─────────┘
```

## Backend - Clean Architecture

### Camadas

1. **InovaSaude.API**
   - Controllers HTTP
   - Middleware de autenticação
   - Configuração de serviços
   - Swagger/OpenAPI

2. **InovaSaude.Application**
   - DTOs (Data Transfer Objects)
   - Services (lógica de negócio)
   - Interfaces de serviços
   - Validações

3. **InovaSaude.Core**
   - Entidades de domínio
   - Interfaces de repositórios
   - Enumerações
   - Lógica de negócio central

4. **InovaSaude.Infrastructure**
   - Implementação de repositórios
   - DbContext (Entity Framework)
   - Serviços de infraestrutura (JWT, etc.)
   - Migrations

### Entidades Principais

```
┌──────────────┐
│   Usuario    │
├──────────────┤
│ Id (Guid)    │
│ Nome         │
│ Email        │
│ Cpf          │
│ MunicipioId  │
│ UbsId        │
│ Roles        │
└──────────────┘
       │
       │ 1:N
       ▼
┌──────────────┐
│   Despesa    │
├──────────────┤
│ Id           │
│ Valor        │
│ Data         │
│ Descricao    │
│ Status       │
│ UbsId        │
│ CategoriaId  │
│ UsuarioId    │
└──────────────┘
       │
       │ N:1
       ▼
┌──────────────┐       ┌──────────────┐
│     UBS      │       │  Categoria   │
├──────────────┤       ├──────────────┤
│ Id           │       │ Id           │
│ Nome         │       │ Nome         │
│ Endereco     │       │ Descricao    │
│ MunicipioId  │       │ Codigo       │
└──────────────┘       └──────────────┘
       │
       │ N:1
       ▼
┌──────────────┐
│  Municipio   │
├──────────────┤
│ Id           │
│ Nome         │
│ Estado       │
│ CodigoIbge   │
└──────────────┘
```

## Frontend - React Architecture

### Estrutura de Pastas

```
src/
├── components/      # Componentes reutilizáveis
│   ├── Layout/      # Layout principal
│   └── ...
├── pages/           # Páginas da aplicação
│   ├── Login/
│   ├── Dashboard/
│   ├── UBS/
│   └── Despesas/
├── contexts/        # React Contexts
│   └── AuthContext.tsx
├── services/        # Serviços de API
│   ├── api.ts
│   ├── authService.ts
│   ├── ubsService.ts
│   └── despesaService.ts
├── hooks/           # Custom hooks
├── types/           # TypeScript types
└── utils/           # Utilitários
```

### Fluxo de Dados

```
User Action
    │
    ▼
React Component
    │
    ▼
Service Layer (Axios)
    │
    ▼
Backend API
    │
    ▼
Database
    │
    ▼
Response
    │
    ▼
React Query Cache
    │
    ▼
Component Update
```

## Autenticação e Segurança

### Fluxo de Autenticação

```
1. Login Request
   ├─▶ POST /api/auth/login
   │   { email, password }
   │
2. Backend Validation
   ├─▶ ASP.NET Identity
   │   Verifica credenciais
   │
3. Token Generation
   ├─▶ JWT Token Service
   │   Gera access token
   │
4. Response
   └─▶ { token, user, expiresAt }
```

### Segurança Implementada

- **JWT Tokens**: Autenticação stateless
- **Password Hashing**: bcrypt via ASP.NET Identity
- **CORS**: Configurado para origens permitidas
- **HTTPS**: Obrigatório em produção
- **SQL Injection**: Proteção via EF Core
- **XSS**: Proteção via React
- **Role-based Authorization**: Admin, Gestor, Coordenador
- **Soft Delete**: Dados nunca são deletados fisicamente

## API Design

### REST Principles

- Recursos nomeados com substantivos (não verbos)
- Métodos HTTP apropriados (GET, POST, PUT, DELETE)
- Status codes HTTP corretos
- Versionamento via URL quando necessário

### Padrões de Response

**Sucesso (200 OK):**
```json
{
  "id": "guid",
  "nome": "UBS Centro",
  ...
}
```

**Erro (400 Bad Request):**
```json
{
  "message": "Validation error"
}
```

**Erro (401 Unauthorized):**
```json
{
  "message": "Invalid credentials"
}
```

## Database Schema

### Relacionamentos

- **Usuario** 1:N **Despesa**
- **UBS** 1:N **Despesa**
- **Categoria** 1:N **Despesa**
- **Municipio** 1:N **UBS**
- **Municipio** 1:N **Usuario**
- **UBS** 1:N **Usuario** (Coordenadores)

### Índices

- Usuario: Email (unique), Cpf (unique)
- UBS: Cnes (unique)
- Municipio: CodigoIbge (unique)

### Soft Delete

Todas as entidades principais têm `IsDeleted` flag para soft delete.

## Infraestrutura

### Docker Containers

```
┌─────────────────┐
│    Frontend     │
│   (Nginx:80)    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐     ┌──────────────┐
│    Backend      │────▶│  PostgreSQL  │
│   (.NET:5000)   │     │   (:5432)    │
└─────────────────┘     └──────────────┘
```

### Volumes

- `postgres_data`: Dados do PostgreSQL
- `backend/uploads`: Arquivos enviados
- `backend/logs`: Logs da aplicação

## Observabilidade

### Logging

- **Serilog** para structured logging
- Níveis: Debug, Info, Warning, Error
- Outputs: Console, File
- Rotação diária de logs

### Health Checks

```
GET /health
{
  "status": "healthy",
  "database": "ok",
  "timestamp": "2026-01-06T01:00:00Z"
}
```

### Métricas (Futuro)

- Application Insights (Azure)
- Prometheus + Grafana
- Alertas automáticos

## Escalabilidade

### Fase Atual (MVP)

- Monolítico
- Instância única
- ~50 usuários

### Fase 2 (~500 usuários)

- Load balancer
- Múltiplas instâncias da API
- Redis cache
- CDN para assets

### Fase 3 (~5000 usuários)

- Microserviços
- Message queue (RabbitMQ)
- Elasticsearch para logs
- Banco replicado
- Auto-scaling

## Performance

### Targets (Fase MVP)

- Tempo de resposta: < 2s
- Throughput: 100 req/s
- Disponibilidade: 99%

### Otimizações Implementadas

- Eager loading no EF Core
- Paginação em listagens
- Compressão gzip (nginx)
- Cache de assets estáticos
- Índices de banco de dados

## Backup e Recovery

### Estratégia

- **Backup diário** do PostgreSQL
- **Retenção**: 7 dias (MVP), 30 dias (produção)
- **Storage redundante** para comprovantes
- **Backup de configurações** em Git

### Recovery Time Objective (RTO)

- MVP: 4 horas
- Produção: 1 hora

## Compliance

### LGPD

- Consentimento explícito
- Dados mínimos necessários
- Direito ao esquecimento (soft delete)
- Logs de acesso a dados sensíveis
- Retenção de 5 anos para auditoria

### Auditoria

Toda ação crítica é registrada em `AuditLog`:
- Quem realizou
- Quando realizou
- O que foi alterado
- Endereço IP

## Deploy

### Ambientes

1. **Development**: Local
2. **Staging**: Docker em VPS (opcional)
3. **Production**: Azure/Heroku/VPS

### CI/CD

GitHub Actions:
1. Build backend
2. Build frontend
3. Run tests
4. Security scan
5. Docker build
6. Deploy (se main branch)

## Custos Estimados (MVP)

### Opção Cloud Econômica

- Azure App Service B1: ~R$ 50/mês
- Azure SQL Basic: ~R$ 25/mês
- Azure Storage: ~R$ 5/mês
- **Total**: ~R$ 80/mês

### Opção VPS

- VPS 2GB RAM: ~R$ 30-50/mês
- PostgreSQL incluído
- **Total**: ~R$ 40/mês

## Tecnologias e Versões

| Componente | Tecnologia | Versão |
|------------|-----------|---------|
| Backend Runtime | .NET | 8.0 |
| Backend Framework | ASP.NET Core | 8.0 |
| ORM | Entity Framework Core | 8.0 |
| Database | PostgreSQL | 15+ |
| Frontend Runtime | Node.js | 20+ |
| Frontend Framework | React | 18+ |
| Build Tool | Vite | 5+ |
| Language | TypeScript | 5+ |
| State Management | React Query | 5+ |
| HTTP Client | Axios | 1+ |
| Container | Docker | 24+ |
| Web Server | Nginx | 1.25+ |

## Próximos Passos

1. ✅ Estrutura base do projeto
2. ✅ Autenticação JWT
3. ✅ CRUD de UBS e Despesas
4. ✅ Frontend React
5. ✅ Docker configuration
6. [ ] Relatórios avançados
7. [ ] Testes automatizados
8. [ ] Deploy em produção
9. [ ] Monitoramento
10. [ ] App mobile
