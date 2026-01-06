# Inova + Saúde

Sistema de análise e gerenciamento de gastos por UBS para prefeituras.

## 🎯 Objetivo

Plataforma web para gestão financeira de Unidades Básicas de Saúde (UBS), permitindo análise, controle e otimização de despesas municipais na área da saúde.

## 🏗️ Arquitetura

### Stack Tecnológica

**Frontend:**
- React 18+
- TypeScript
- Vite
- TailwindCSS
- React Query
- React Hook Form
- Zod (validação)

**Backend:**
- Node.js 20+
- TypeScript
- Express
- PostgreSQL 15+
- Prisma ORM
- JWT Authentication

**Infraestrutura:**
- Docker & Docker Compose
- Nginx (reverse proxy)

## 📁 Estrutura do Projeto

```
InovaSaude/
├── docs/                    # Documentação
├── frontend/               # Aplicação React
├── backend/                # API Node.js
├── database/               # Scripts e migrations
├── docker/                 # Configurações Docker
└── infrastructure/         # Configs de infraestrutura
```

## 🚀 Início Rápido

### Pré-requisitos
- Node.js 20+
- Docker & Docker Compose
- PostgreSQL 15+

### Instalação

```bash
# Clone o repositório
git clone https://github.com/Benjamim0259/InovaSaude.git
cd InovaSaude

# Configure as variáveis de ambiente
cp .env.example .env

# Suba os containers
docker-compose up -d

# Instale as dependências
npm install

# Execute as migrations
npm run migrate

# Inicie o desenvolvimento
npm run dev
```

## 📊 Funcionalidades Principais

### Módulos do Sistema

1. **Gestão de UBS**
   - Cadastro e gerenciamento de unidades
   - Hierarquia organizacional
   - Indicadores por unidade

2. **Controle de Despesas**
   - Lançamento manual de despesas
   - Importação em massa (CSV/Excel)
   - Categorização automática
   - Anexo de documentos

3. **Análise e Relatórios**
   - Dashboards interativos
   - Comparativos entre UBS
   - Séries históricas
   - Alertas de desvios

4. **Administração**
   - Gestão de usuários e permissões
   - Auditoria de ações
   - Configurações do sistema

## 👥 Perfis de Usuário

- **Administrador:** Acesso total ao sistema
- **Coordenador:** Gestão de UBS específicas
- **Analista:** Visualização e relatórios
- **Operador:** Lançamento de despesas

## 📝 Licença

Este projeto está sob licença proprietária.

## 🤝 Contribuindo

Contribuições são bem-vindas! Por favor, leia o guia de contribuição.

## 📧 Contato

Para mais informações, entre em contato através das issues do GitHub.