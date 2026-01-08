# Quick Start Guide - Local Docker Setup

Este guia permite rodar o projeto completo localmente com Docker sem hospedagem no Render.

## Pré-requisitos

- Docker & Docker Compose instalados
- Git (opcional se tiver o projeto já clonado)

## Como Rodar Localmente

### 1. Ative o modo debug no frontend (opcional)

Se quiser testar frontend sem backend:

```bash
# No arquivo frontend/.env, garanta:
VITE_DEBUG_MODE=true
VITE_API_URL=http://localhost:5000/api
```

### 2. Suba os containers com Docker Compose

```bash
cd "c:\Users\WTINFO PC\source\repos\InovaSaude"
docker-compose up -d
```

Isso vai:
- ✅ Criar container PostgreSQL (porta 5432)
- ✅ Construir e rodar backend .NET (porta 5000)
- ✅ Construir e rodar frontend React (porta 80/3000)

### 3. Aguarde 30-60 segundos pelo build

Os primeiros 30 segundos podem ser lentos na primeira build.

### 4. Acesse a aplicação

- **Frontend:** http://localhost
- **Backend API:** http://localhost:5000
- **Health Check:** http://localhost:5000/health
- **Database:** localhost:5432

### 5. Login

Use as credenciais do seed (se o banco foi migrado):

- Email: admin@inovasaude.com.br
- Senha: admin123

Ou teste com modo debug:
- Qualquer email / qualquer senha

## Comandos Úteis

### Ver logs
```bash
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f postgres
```

### Parar containers
```bash
docker-compose down
```

### Remover volumes (limpar DB)
```bash
docker-compose down -v
```

### Reconstruir após mudanças
```bash
docker-compose up --build
```

## Troubleshooting

### Porta já em uso
Se a porta 80, 5000 ou 5432 já está em uso, modifique o `docker-compose.yml`:

```yaml
ports:
  - "8080:80"  # Frontend em 8080
  - "5001:5000" # Backend em 5001
  - "5433:5432" # DB em 5433
```

### Banco de dados não carrega migrations
```bash
docker-compose exec backend npx prisma migrate dev --name init
docker-compose exec backend npx prisma db seed
```

### Frontend não conecta com backend
Verifique se:
- Backend está rodando: `docker ps`
- `VITE_API_URL` está correto no `.env` do frontend
- Network está conectada: `docker network ls`

## Deploy no Render depois

Quando estiver pronto para hospedar:

1. Siga as instruções em [DEPLOY_RENDER.md](DEPLOY_RENDER.md)
2. O render.yaml já está preparado para:
   - Backend .NET via Docker
   - Frontend como static site
   - PostgreSQL gerenciado pelo Render

---

**Nota:** O free tier do Render hiberna após 15 min sem tráfego. O frontend já possui retry e loading states para lidar com isso.
