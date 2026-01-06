# InovaSaude 🏥

Sistema de análise e gerenciamento de gastos por UBS para prefeituras.

## Arquitetura

- **Backend**: Node.js + TypeScript + Express
- **Frontend**: React + TypeScript + Vite
- **Banco de Dados**: PostgreSQL
- **Cache**: Redis
- **Autenticação**: JWT + 2FA
- **Deploy**: Docker + Docker Compose

## Estrutura do Projeto

```
InovaSaude/
├── backend/          # API REST em Node.js
├── frontend/         # Interface React
├── database/         # Scripts e migrations
├── docker/           # Configurações Docker
└── docs/            # Documentação
```

## Como Executar

```bash
# Clone o repositório
git clone https://github.com/Benjamim0259/InovaSaude.git
cd InovaSaude

# Execute com Docker
docker-compose up -d
```

## Funcionalidades

- ✅ Gestão de despesas por UBS
- ✅ Importação em massa de dados
- ✅ Relatórios e dashboards
- ✅ Controle de acesso por perfil
- ✅ Autenticação 2FA

## Status

🚧 Em desenvolvimento inicial
