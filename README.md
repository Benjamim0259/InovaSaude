# InovaSaude 🏥

Sistema de análise e gerenciamento de gastos por UBS (Unidade Básica de Saúde) para prefeituras.

## Arquitetura

Plataforma web para gestão financeira de Unidades Básicas de Saúde, permitindo análise, controle e otimização de despesas municipais na área da saúde.

## 🏗️ Arquitetura

### Stack Tecnológica

**Backend:**
- C# / .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Serilog (Logging)
- Clean Architecture

**Frontend:**
- **Blazor WebAssembly (em migração)**
- React 18+ com TypeScript (legado)
- Vite
- React Router
- React Query
- Axios
- React Hook Form + Zod

**Infraestrutura:**
- Docker & Docker Compose
- Nginx (reverse proxy)
- GitHub Actions (CI/CD)

## 📁 Estrutura do Projeto

```
InovaSaude/
├── backend/                # Backend .NET
│   ├── src/
│   │   ├── InovaSaude.API/           # Web API
│   │   ├── InovaSaude.Core/          # Domain entities
│   │   ├── InovaSaude.Application/   # Services & DTOs
│   │   └── InovaSaude.Infrastructure/ # Data & Repositories
│   └── tests/
│       └── InovaSaude.Tests/         # Unit tests
├── frontend/               # Frontend React (legado)
├── frontend-blazor/        # Frontend Blazor (em migração)
│   ├── src/
│   │   ├── components/     # React components
│   │   ├── pages/          # Page components
│   │   ├── services/       # API services
│   │   ├── contexts/       # React contexts
│   │   ├── hooks/          # Custom hooks
│   │   ├── types/          # TypeScript types
│   │   └── utils/          # Utility functions
│   └── public/
└── docker-compose.yml      # Docker configuration
```

## 🚀 Início Rápido

### Pré-requisitos
- .NET 8 SDK
- Node.js 20+
- PostgreSQL 15+ (ou Docker)
- Docker & Docker Compose (opcional)

### Opção 1: Executar com Docker

```bash
# Clone o repositório
git clone https://github.com/Benjamim0259/InovaSaude.git
cd InovaSaude

# Suba os containers
docker-compose up -d
# Execute com Docker
docker-compose up -d

# Execute as migrations do Prisma
docker-compose exec backend npx prisma migrate dev

# Acesse a aplicação
# Frontend: http://localhost:3000
# Backend: http://localhost:4000
# Health check: http://localhost:4000/health
```

### Instalação Manual

#### Backend

```bash
cd backend

# Instale as dependências
npm install

# Configure as variáveis de ambiente
cp .env.example .env

# Gere o Prisma Client
npx prisma generate

# Execute as migrations
npx prisma migrate dev

# Inicie o servidor de desenvolvimento
npm run dev
```

#### Frontend (React - legado)

```bash
cd frontend

# Acesse a aplicação
# Frontend: http://localhost
# Backend API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Opção 2: Executar localmente

#### Backend

```bash
# Navegue até o diretório do backend
cd backend

# Restaure as dependências
dotnet restore

# Configure a string de conexão no appsettings.json
# Ajuste a ConnectionString para seu PostgreSQL local

# Execute as migrations (quando criadas)
# dotnet ef database update --project src/InovaSaude.Infrastructure

# Execute a API
dotnet run --project src/InovaSaude.API

# A API estará disponível em http://localhost:5000
# Swagger UI em http://localhost:5000/swagger
```

#### Frontend

```bash
# Navegue até o diretório do frontend
cd frontend

# Instale as dependências
npm install

# Configure as variáveis de ambiente
cp .env.example .env

# Inicie o servidor de desenvolvimento
npm run dev

# O frontend estará disponível em http://localhost:5173

#### Frontend (Blazor - em migração)

Consulte as orientações em [frontend-blazor/README.md](frontend-blazor/README.md).
```

## 📊 Funcionalidades Principais

### Módulos do Sistema

1. **Gestão de UBS**
   - Cadastro e gerenciamento de unidades
   - Associação com municípios
   - Atribuição de coordenadores

2. **Controle de Despesas**
   - Lançamento de despesas
   - Categorização
   - Upload de comprovantes
   - Aprovação e rejeição
   - Status (Pendente, Aprovada, Rejeitada)

3. **Análise e Relatórios**
   - Dashboard com indicadores
   - Relatórios por UBS
   - Relatórios por categoria
   - Comparativos por período
   - Exportação (Excel/PDF - futuro)

4. **Administração**
   - Gestão de usuários e permissões
   - Auditoria de ações
   - Logs do sistema

- **Autenticação:** JWT com expiração configurável
- **Senhas:** Hash com Bcrypt (10 rounds)
- **Rate Limiting:** 100 requisições/minuto por IP
- **CORS:** Configurado para origens permitidas
- **Headers de Segurança:** Helmet.js
- **Validação de Dados:** Zod schemas
- **Sanitização:** Inputs sanitizados
- **Auditoria:** Logs de ações críticas

## 📡 API Endpoints

- **Admin:** Acesso total ao sistema
- **Gestor:** Gestão de UBS e despesas do município
- **Coordenador:** Lançamento de despesas da UBS específica

## 🔐 Autenticação e Segurança

- JWT (JSON Web Tokens) para autenticação
- Senhas criptografadas com bcrypt
- Autorização baseada em roles (Admin, Gestor, Coordenador)
- CORS configurado
- HTTPS obrigatório em produção
- Proteção contra SQL Injection (EF Core)
- Proteção contra XSS (React)

## 📡 API Endpoints

### Autenticação
```
POST   /api/auth/login      - Login
POST   /api/auth/register   - Registrar usuário (Admin only)
GET    /api/auth/me         - Obter usuário atual
POST   /api/auth/refresh    - Refresh token
```

### UBS
```
GET    /api/ubs             - Listar UBS
GET    /api/ubs/{id}        - Obter UBS por ID
POST   /api/ubs             - Criar UBS
PUT    /api/ubs/{id}        - Atualizar UBS
DELETE /api/ubs/{id}        - Deletar UBS
```

### Despesas
```
GET    /api/despesas                    - Listar despesas
GET    /api/despesas/{id}               - Obter despesa por ID
POST   /api/despesas                    - Criar despesa
PUT    /api/despesas/{id}               - Atualizar despesa
DELETE /api/despesas/{id}               - Deletar despesa
POST   /api/despesas/{id}/comprovante   - Upload de comprovante
```

### Health Check
```
GET    /health              - Status da API e banco de dados
```

## 🛠️ Desenvolvimento

### Rodando Testes

```bash
# Backend
cd backend
dotnet test

# Frontend
cd frontend
npm test
```

### Linting e Formatação

```bash
# Backend (use ferramentas do .NET)
dotnet format

# Frontend
npm run lint
```

### Build para Produção

```bash
# Backend
cd backend
dotnet publish -c Release -o ./publish

# Frontend
cd frontend
npm run build
```

## 🚢 Deploy

### Opção 1: Azure App Service

1. Configure Azure App Service e Azure SQL Database
2. Configure variáveis de ambiente no App Service
3. Use GitHub Actions para deploy automático

### Opção 2: Heroku

1. Configure Heroku Postgres
2. Configure variáveis de ambiente
3. Faça deploy via Git ou Docker

### Opção 3: Docker em VPS

```bash
# No servidor
git clone https://github.com/Benjamim0259/InovaSaude.git
cd InovaSaude
docker-compose up -d
```

## 📝 Configuração

### Variáveis de Ambiente (Backend)

```env
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=inovasaude;Username=postgres;Password=postgres
Jwt__SecretKey=your-secret-key-min-32-characters-long
Jwt__Issuer=InovaSaude
Jwt__Audience=InovaSaude
Jwt__ExpirationMinutes=60
Cors__AllowedOrigins=http://localhost:3000
```

### Variáveis de Ambiente (Frontend)

```env
VITE_API_URL=http://localhost:5000/api
```

## 📈 Roadmap

### Fase 1 (MVP) - Concluído
- ✅ Estrutura backend com Clean Architecture
- ✅ Autenticação JWT
- ✅ CRUD de UBS e Despesas
- ✅ Frontend React com TypeScript
- ✅ Login e rotas protegidas
- ✅ Dashboard básico

### Fase 2 (Em desenvolvimento)
- [ ] Relatórios avançados
- [ ] Gráficos e visualizações
- [ ] Export para Excel/PDF
- [ ] Testes automatizados
- [ ] CI/CD completo

### Fase 3 (Futuro)
- [ ] App mobile (React Native)
- [ ] Notificações push
- [ ] Integração com sistemas da prefeitura
- [ ] Machine Learning para detecção de anomalias
- [ ] Dashboard avançado com BI

## 🤝 Contribuindo

Contribuições são bem-vindas! Por favor:

1. Faça um Fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob licença proprietária.
Para mais informações, entre em contato através das issues do GitHub.

## 🙏 Agradecimentos

Para mais informações, entre em contato através das issues do GitHub.

---

**Desenvolvido com ❤️ para melhorar a gestão pública de saúde**
