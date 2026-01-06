# Setup Guide - InovaSaúde

## Guia Rápido de Configuração

### 1. Pré-requisitos

Certifique-se de ter instalado:
- Node.js 20+ ([Download](https://nodejs.org/))
- Docker Desktop ([Download](https://www.docker.com/products/docker-desktop/))
- Git ([Download](https://git-scm.com/))

### 2. Clone o Repositório

```bash
git clone https://github.com/Benjamim0259/InovaSaude.git
cd InovaSaude
```

### 3. Configuração de Ambiente

```bash
# Copie o arquivo de exemplo
cp .env.example .env

# Edite o arquivo .env e configure suas variáveis
# Especialmente importante: JWT_SECRET (use uma string aleatória longa)
```

### 4. Inicie com Docker (RECOMENDADO)

#### 4.1. Suba os containers

```bash
docker-compose up -d
```

Isso irá:
- ✅ Criar o container do PostgreSQL
- ✅ Criar o container do backend
- ✅ Criar o container do frontend
- ✅ Configurar a rede entre os containers

#### 4.2. Execute as migrations e seed

```bash
# Entre no container do backend
docker-compose exec backend sh

# Execute as migrations
npx prisma migrate dev --name init

# Execute o seed para criar dados iniciais
npx prisma db seed

# Saia do container
exit
```

#### 4.3. Acesse a aplicação

- **Frontend:** http://localhost:3000
- **Backend API:** http://localhost:4000
- **Health Check:** http://localhost:4000/health

#### 4.4. Faça login

Use as credenciais criadas pelo seed:

- **Admin:** admin@inovasaude.com.br / admin123
- **Coordenador 1:** maria.silva@inovasaude.com.br / senha123
- **Coordenador 2:** joao.santos@inovasaude.com.br / senha123
- **Gestor:** carlos.oliveira@inovasaude.com.br / senha123

### 5. Instalação Manual (Alternativa)

Se preferir não usar Docker:

#### 5.1. Configure o PostgreSQL

```bash
# Instale o PostgreSQL 15+
# Crie o banco de dados
createdb inovasaude
```

#### 5.2. Backend

```bash
cd backend

# Instale as dependências
npm install

# Gere o Prisma Client
npx prisma generate

# Execute as migrations
npx prisma migrate dev

# Execute o seed
npm run prisma:seed

# Inicie o servidor
npm run dev
```

O backend estará rodando em: http://localhost:4000

#### 5.3. Frontend

```bash
cd frontend

# Instale as dependências
npm install

# Inicie o servidor de desenvolvimento
npm run dev
```

O frontend estará rodando em: http://localhost:3000

### 6. Verificação

#### 6.1. Teste o Backend

```bash
# Health check
curl http://localhost:4000/health

# Login
curl -X POST http://localhost:4000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@inovasaude.com.br","senha":"admin123"}'
```

#### 6.2. Teste o Frontend

Abra http://localhost:3000 no navegador e faça login.

### 7. Prisma Studio (Opcional)

Para visualizar e editar dados do banco:

```bash
cd backend
npx prisma studio
```

Acesse: http://localhost:5555

### 8. Comandos Úteis

#### Docker

```bash
# Ver logs
docker-compose logs -f

# Ver logs apenas do backend
docker-compose logs -f backend

# Parar os containers
docker-compose down

# Parar e remover volumes (limpa o banco)
docker-compose down -v

# Reconstruir os containers
docker-compose up -d --build
```

#### Backend

```bash
cd backend

# Executar testes
npm run test

# Lint do código
npm run lint

# Formatar código
npm run format

# Build para produção
npm run build

# Criar nova migration
npx prisma migrate dev --name nome_da_migration

# Resetar banco de dados
npx prisma migrate reset
```

#### Frontend

```bash
cd frontend

# Build para produção
npm run build

# Preview do build
npm run preview

# Lint do código
npm run lint
```

### 9. Estrutura de Desenvolvimento

```
InovaSaude/
├── backend/              # API Node.js
│   ├── src/
│   │   ├── modules/     # Módulos de negócio
│   │   ├── config/      # Configurações
│   │   └── shared/      # Código compartilhado
│   └── prisma/          # Schema e migrations
├── frontend/            # App React
│   └── src/
│       ├── pages/       # Páginas
│       ├── components/  # Componentes
│       └── services/    # Serviços de API
└── docker-compose.yml   # Orquestração
```

### 10. Resolução de Problemas

#### Erro de conexão com banco de dados

```bash
# Verifique se o PostgreSQL está rodando
docker-compose ps

# Recrie o container do banco
docker-compose down
docker-compose up -d postgres
```

#### Erro "Cannot find module @prisma/client"

```bash
cd backend
npx prisma generate
```

#### Porta já em uso

```bash
# Altere as portas no arquivo .env
APP_PORT=3001
API_PORT=4001
```

#### Frontend não conecta ao backend

```bash
# Verifique o arquivo frontend/.env
# Deve ter: VITE_API_URL=http://localhost:4000/api
```

### 11. Próximos Passos

1. ✅ Configure o sistema
2. ✅ Faça login como admin
3. ✅ Explore o dashboard
4. ✅ Crie uma nova UBS
5. ✅ Adicione usuários
6. ✅ Cadastre despesas

### 12. Suporte

- 📧 Issues: https://github.com/Benjamim0259/InovaSaude/issues
- 📚 Documentação: README.md
- 💬 Discussões: GitHub Discussions
