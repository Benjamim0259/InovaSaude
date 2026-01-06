# InovaSaude 🏥

Sistema de análise e gerenciamento de gastos por UBS (Unidade Básica de Saúde) para prefeituras.

## Arquitetura

Plataforma web para gestão financeira de Unidades Básicas de Saúde, permitindo análise, controle e otimização de despesas municipais na área da saúde.

## 🏗️ Arquitetura

### Stack Tecnológica

**Frontend:**
- React 18+ com TypeScript
- Vite
- TailwindCSS
- React Query (TanStack Query)
- React Hook Form + Zod
- Axios
- React Router v6

**Backend:**
- Node.js 20+ com TypeScript
- Express.js
- PostgreSQL 15+
- Prisma ORM
- JWT Authentication
- Bcrypt
- Winston (logs)
- Multer (upload)
- Helmet (security)

**Infraestrutura:**
- Docker & Docker Compose
- PostgreSQL 15

## 📁 Estrutura do Projeto

```
InovaSaude/
├── backend/                  # API Node.js + TypeScript
│   ├── src/
│   │   ├── config/          # Configurações (DB, Auth, Logger)
│   │   ├── modules/         # Módulos da aplicação
│   │   │   ├── auth/        # Autenticação e autorização
│   │   │   ├── despesas/    # Gestão de despesas
│   │   │   ├── ubs/         # Gestão de UBS
│   │   │   ├── usuarios/    # Gestão de usuários
│   │   │   ├── relatorios/  # Relatórios e dashboards
│   │   │   └── importacao/  # Importação em massa
│   │   ├── shared/          # Middlewares e utilitários
│   │   │   ├── middlewares/ # Auth, Error, Validation
│   │   │   └── utils/       # Funções auxiliares
│   │   └── app.ts          # Entry point
│   ├── prisma/
│   │   └── schema.prisma   # Schema do banco de dados
│   ├── tests/              # Testes automatizados
│   └── package.json
├── frontend/               # Aplicação React
│   ├── src/
│   │   ├── components/    # Componentes React
│   │   │   ├── auth/      # Componentes de autenticação
│   │   │   ├── dashboard/ # Componentes do dashboard
│   │   │   ├── despesas/  # Componentes de despesas
│   │   │   ├── ubs/       # Componentes de UBS
│   │   │   └── shared/    # Componentes compartilhados
│   │   ├── pages/         # Páginas da aplicação
│   │   ├── services/      # Chamadas API
│   │   ├── hooks/         # React hooks customizados
│   │   ├── contexts/      # Contextos React
│   │   ├── utils/         # Funções auxiliares
│   │   └── types/         # Tipos TypeScript
│   └── package.json
├── docker-compose.yml     # Orquestração de containers
└── README.md
```

## 🚀 Início Rápido

### Pré-requisitos

- Node.js 20+
- Docker & Docker Compose
- PostgreSQL 15+ (ou usar Docker)

### Instalação com Docker (Recomendado)

```bash
# Clone o repositório
git clone https://github.com/Benjamim0259/InovaSaude.git
cd InovaSaude

# Execute com Docker
docker-compose up -d
<<<<<<< copilot/create-initial-structure-ubs-system

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

#### Frontend

```bash
cd frontend

# Instale as dependências
npm install

# Configure as variáveis de ambiente
cp .env.example .env

# Inicie o servidor de desenvolvimento
npm run dev
```

## 📊 Funcionalidades Principais

### Módulos do Sistema

#### 1. **Autenticação e Autorização**
- Login com email/senha
- JWT para autenticação
- 4 perfis de usuário: ADMIN, COORDENADOR, GESTOR, AUDITOR
- Recuperação de senha
- Controle de sessão

#### 2. **Gestão de UBS**
- CRUD completo de unidades
- Associação com coordenadores
- Status (Ativa, Inativa, Em Manutenção)
- Visualização de despesas por UBS
- Indicadores por unidade

#### 3. **Controle de Despesas**
- CRUD completo de despesas
- Workflow de aprovação (Pendente → Aprovada → Paga → Rejeitada)
- Categorização (Pessoal, Material, Serviço, Equipamento, etc.)
- Tipos: Fixa, Variável, Eventual
- Upload de anexos (comprovantes, notas fiscais)
- Filtros avançados (UBS, período, categoria, status)
- Associação com fornecedores
- Histórico de mudanças de status

#### 4. **Gestão de Fornecedores**
- Cadastro completo
- CNPJ, Razão Social, Contatos
- Status (Ativo, Inativo, Bloqueado)

#### 5. **Análise e Relatórios**
- Dashboard interativo
- Gastos por UBS
- Gastos por categoria
- Comparativos mensais/anuais
- Indicadores financeiros
- Alertas de desvios (futuro)

#### 6. **Importação em Massa**
- Upload de arquivos CSV/XLSX
- Validação de dados
- Preview antes da importação
- Processamento assíncrono
- Template para download
- Log de erros e sucessos

#### 7. **Administração**
- Gestão de usuários e permissões
- Auditoria de ações críticas
- Logs estruturados

## 👥 Perfis de Usuário

| Perfil | Descrição | Permissões |
|--------|-----------|------------|
| **ADMIN** | Administrador do sistema | Acesso total, gestão de usuários, configurações |
| **COORDENADOR** | Coordenador de UBS | Gestão de despesas da sua UBS |
| **GESTOR** | Gestor municipal | Visualização e aprovação de despesas, relatórios |
| **AUDITOR** | Auditor | Visualização de dados, relatórios, sem poder de edição |

## 🔐 Segurança

- **Autenticação:** JWT com expiração configurável
- **Senhas:** Hash com Bcrypt (10 rounds)
- **Rate Limiting:** 100 requisições/minuto por IP
- **CORS:** Configurado para origens permitidas
- **Headers de Segurança:** Helmet.js
- **Validação de Dados:** Zod schemas
- **Sanitização:** Inputs sanitizados
- **Auditoria:** Logs de ações críticas

## 📡 API Endpoints

### Auth
```
POST   /api/auth/login          # Login
POST   /api/auth/register       # Registro (apenas ADMIN)
POST   /api/auth/logout         # Logout
POST   /api/auth/refresh        # Refresh token
POST   /api/auth/forgot-password # Recuperação de senha
POST   /api/auth/reset-password  # Reset de senha
```

### Despesas
```
GET    /api/despesas            # Listar (com filtros)
GET    /api/despesas/:id        # Buscar por ID
POST   /api/despesas            # Criar
PUT    /api/despesas/:id        # Atualizar
DELETE /api/despesas/:id        # Deletar
POST   /api/despesas/:id/aprovar   # Aprovar
POST   /api/despesas/:id/rejeitar  # Rejeitar
POST   /api/despesas/:id/pagar     # Marcar como paga
```

### UBS
```
GET    /api/ubs                 # Listar
GET    /api/ubs/:id             # Buscar por ID
POST   /api/ubs                 # Criar (ADMIN/GESTOR)
PUT    /api/ubs/:id             # Atualizar (ADMIN/GESTOR)
DELETE /api/ubs/:id             # Deletar (ADMIN)
```

### Usuários
```
GET    /api/usuarios            # Listar (ADMIN)
GET    /api/usuarios/:id        # Buscar por ID
POST   /api/usuarios            # Criar (ADMIN)
PUT    /api/usuarios/:id        # Atualizar (ADMIN)
DELETE /api/usuarios/:id        # Deletar (ADMIN)
```

### Relatórios
```
GET    /api/relatorios/dashboard           # Visão geral
GET    /api/relatorios/gastos-por-ubs      # Gastos por UBS
GET    /api/relatorios/gastos-por-categoria # Gastos por categoria
GET    /api/relatorios/comparativo-mensal   # Comparativo mensal
```

### Importação
```
POST   /api/importacao/upload   # Upload de arquivo
GET    /api/importacao/template # Download template
GET    /api/importacao/lotes    # Listar lotes
```

## 🗄️ Modelo de Dados

### Entidades Principais

- **Usuario:** Usuários do sistema
- **UBS:** Unidades Básicas de Saúde
- **Despesa:** Despesas/gastos
- **Fornecedor:** Fornecedores
- **Categoria:** Categorias de despesas
- **Anexo:** Arquivos anexados às despesas
- **HistoricoDespesa:** Histórico de mudanças
- **LogAuditoria:** Logs de auditoria
- **ImportacaoLote:** Lotes de importação

Ver `backend/prisma/schema.prisma` para detalhes completos.

## 🧪 Testes

```bash
# Backend
cd backend
npm run test
npm run test:coverage

# Frontend
cd frontend
npm run test
```

## 📝 Scripts Disponíveis

### Backend
```bash
npm run dev          # Desenvolvimento
npm run build        # Build para produção
npm run start        # Iniciar produção
npm run test         # Executar testes
npm run lint         # Lint do código
npm run lint:fix     # Corrigir problemas de lint
npm run prisma:generate  # Gerar Prisma Client
npm run prisma:migrate   # Executar migrations
npm run prisma:studio    # Abrir Prisma Studio
```

### Frontend
```bash
npm run dev          # Desenvolvimento
npm run build        # Build para produção
npm run preview      # Preview do build
npm run lint         # Lint do código
npm run lint:fix     # Corrigir problemas de lint
```

## 🔧 Configuração

### Variáveis de Ambiente

Copie `.env.example` para `.env` e configure:

```env
# Application
NODE_ENV=development
APP_PORT=3000
API_PORT=4000

# Database
DATABASE_URL="postgresql://user:password@localhost:5432/inovasaude"
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=inovasaude

# JWT
JWT_SECRET=your-secret-key-change-in-production
JWT_EXPIRES_IN=7d

# Upload
MAX_FILE_SIZE=10485760  # 10MB
UPLOAD_DIR=./uploads

# CORS
CORS_ORIGIN=http://localhost:3000
```

## 📈 Requisitos Não Funcionais

- **Performance:** < 500ms para 95% das requisições
- **Disponibilidade:** 99% uptime (MVP)
- **Segurança:** HTTPS obrigatório em produção
- **Escalabilidade:** Suporta 50 usuários simultâneos (MVP)

## 🚀 Próximos Passos (Roadmap)

- [ ] Notificações por email
- [ ] Dashboard avançado com mais filtros
- [ ] Módulo de orçamentos e planejamento
- [ ] Relatórios customizáveis
- [ ] Exportação de relatórios (PDF/Excel)
- [ ] Integração com sistemas de prefeituras
- [ ] App mobile (React Native)
- [ ] Análise preditiva de gastos
- [ ] Sistema de alertas automáticos

## 📝 Licença

Este projeto está sob licença proprietária.

## 🤝 Contribuindo

Contribuições são bem-vindas! Por favor:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### Convenções de Código

- Use TypeScript
- Siga o ESLint configurado
- Escreva testes para novas funcionalidades
- Documente código complexo
- Use commits semânticos
=======
```

## Funcionalidades

- ✅ Gestão de despesas por UBS
- ✅ Importação em massa de dados
- ✅ Relatórios e dashboards
- ✅ Controle de acesso por perfil
- ✅ Autenticação 2FA
main

## Status

<<<<<<< copilot/create-initial-structure-ubs-system
Para mais informações, entre em contato através das issues do GitHub.

## 🙏 Agradecimentos

Projeto desenvolvido para modernizar a gestão financeira de UBS em municípios brasileiros.
=======
🚧 Em desenvolvimento inicial
main
