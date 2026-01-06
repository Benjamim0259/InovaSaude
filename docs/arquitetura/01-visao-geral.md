# Arquitetura Técnica - Inova + Saúde

## Resumo Executivo

O **InovaSaúde** é uma plataforma web para análise e gerenciamento de gastos por UBS (Unidade Básica de Saúde) destinada a prefeituras. O sistema permite que coordenadores municipais de saúde monitorem e controlem despesas das unidades básicas de forma centralizada e eficiente.

### Objetivos Principais

- **Transparência**: Visibilidade completa dos gastos por UBS
- **Controle**: Gestão centralizada de despesas municipais de saúde
- **Análise**: Relatórios e dashboards para tomada de decisão
- **Rastreabilidade**: Auditoria completa de todas as operações

### Características Técnicas

- **Stack**: Next.js 15, TypeScript, PostgreSQL
- **Arquitetura**: Monolítico modular com separação clara de responsabilidades
- **Segurança**: Autenticação JWT, 2FA, controle de acesso baseado em roles
- **Escalabilidade**: Preparado para ~50 usuários iniciais, escalável conforme demanda

---

## Stack Tecnológica Definida

### Frontend
- **Framework**: Next.js 15 (App Router)
- **Linguagem**: TypeScript
- **Estilização**: Tailwind CSS
- **Biblioteca UI**: Shadcn/ui
- **Gerenciamento de Estado**: Zustand ou Context API
- **Validação**: Zod

### Backend
- **Runtime**: Node.js (via Next.js API Routes)
- **Linguagem**: TypeScript
- **ORM**: Prisma
- **Autenticação**: NextAuth.js ou implementação custom JWT

### Banco de Dados
- **Principal**: PostgreSQL 14+
- **Cache**: Redis (opcional, futuro)

### DevOps & Infra
- **Versionamento**: Git + GitHub
- **CI/CD**: GitHub Actions
- **Deploy**: Vercel (frontend) + Railway/Render (database)
- **Monitoramento**: Sentry (erros) + Vercel Analytics

---

## Próximos Passos Imediatos

1. ✅ Repositório criado
2. 📝 Definir entidades e modelo de dados (ER)
3. 🏗️ Setup inicial do projeto Next.js
4. 🔐 Implementar autenticação básica
5. 📊 Criar CRUD de UBS e Despesas
6. 📈 Dashboards e relatórios

---

**Última atualização**: 2026-01-06
