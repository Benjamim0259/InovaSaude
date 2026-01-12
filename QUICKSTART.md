# 🚀 InovaSaúde - Guia Rápido de Uso

## ⚡ Início Rápido

### Pré-requisitos
- Node.js 20+
- PostgreSQL 15+
- npm ou yarn

### 1️⃣ Instalação
```bash
# Clone o projeto
git clone <repo>
cd InovaSaude

# Instale dependências do backend
cd backend
npm install

# Instale dependências do frontend
cd ../frontend
npm install
cd ..
```

### 2️⃣ Configuração
```bash
# Backend
cd backend
cp .env.example .env

# Edite .env e configure:
# DATABASE_URL=postgresql://postgres:postgres@localhost:5432/inovasaude
# EMAIL_USER=seu_email@gmail.com (opcional)
# EMAIL_PASSWORD=sua_senha_app
```

### 3️⃣ Banco de Dados
```bash
cd backend

# Criar schema
npm run prisma:migrate

# Popular dados de teste
npm run prisma:seed
```

### 4️⃣ Rodar Local
```bash
# Terminal 1 - Backend (porta 4000)
cd backend
npm run dev

# Terminal 2 - Frontend (porta 3000)
cd frontend
npm run dev
```

**Acesso:**
- Frontend: http://localhost:3000
- Backend API: http://localhost:4000

---

## 🔐 Login de Teste

### Opção 1: Admin (Acesso Total)
- **Email**: admin@inovasaude.com.br
- **Senha**: admin123

### Opção 2: Gestor (Aprovações)
- **Email**: carlos.oliveira@inovasaude.com.br
- **Senha**: senha123

### Opção 3: Coordenador (UBS)
- **Email**: maria.silva@inovasaude.com.br
- **Senha**: senha123

---

## 📋 O que Testar?

### ✅ Autenticação
1. Acesse http://localhost:3000
2. Faça login com as credenciais acima
3. Verifique o dashboard

### ✅ Despesas
1. Clique em "Gestão de Despesas" no menu
2. Veja a lista de despesas
3. Clique em "+ Nova Despesa"
4. Preencha o formulário
5. Clique em "Salvar"

### ✅ UBS
1. Clique em "Gestão de UBS" no menu
2. Veja a lista de 5 UBS
3. Clique em "+ Nova UBS"
4. Preencha os dados
5. Clique em "Salvar"

### ✅ Relatórios
1. Clique em "Relatórios" no menu
2. Veja os gráficos e tabelas
3. Use os filtros de data
4. Verifique totalizações

### ✅ Importação (Admin/Gestor)
1. Vá para Despesas
2. Procure por "Importar" (será adicionado em breve)
3. Clique em "Download Template"
4. Preencha a planilha
5. Clique em "Upload"

### ✅ Email (Recuperação)
1. Clique em "Esqueci minha senha" no login
2. Digite seu email
3. Verifique os logs do backend
4. Clique no link de reset (no terminal)
5. Defina uma nova senha

---

## 🛠️ Docker (Opcional)

```bash
# Rodar tudo com Docker
docker-compose up -d

# Acessar
# Frontend: http://localhost:80
# Backend: http://localhost:4000
```

---

## 📊 Estrutura de Dados

### UBS Disponíveis
```
1. UBS Centro - Rua Principal, 100
2. UBS Jardim das Flores - Av. das Flores, 500
3. UBS Vila Esperança - Rua Esperança, 250
4. UBS Alto do Morro - Av. do Morro, 1000
5. UBS São Benedito - Rua São Benedito, 750
```

### Categorias de Despesas
```
- Pessoal (R$ 50.000/mês)
- Material de Consumo (R$ 15.000/mês)
- Serviços (R$ 10.000/mês)
- Equipamentos (R$ 20.000/mês)
- Infraestrutura (R$ 25.000/mês)
- Medicamentos (R$ 35.000/mês)
- Utilidades Públicas (R$ 8.000/mês)
```

### Status de Despesas
```
- PENDENTE (amarelo) - Aguardando aprovação
- APROVADA (azul) - Aprovado
- PAGA (verde) - Já pago
- REJEITADA (vermelho) - Rejeitado
- CANCELADA (cinza) - Cancelado
```

---

## 💡 Dicas

### Recuperação de Senha
- O email vai aparece nos logs do backend (se não estiver configurado)
- Procure por "Recuperação de senha solicitada"
- Copie o token gerado

### Paginação
- Use os botões "Anterior" e "Próxima"
- Mostra 10 itens por página

### Filtros
- Despesas: filtre por status
- Relatórios: filtre por data

### Erros Comuns
- **DATABASE_URL não encontrada**: Verifique o arquivo `.env`
- **Porta já em uso**: Feche outras instâncias ou mude a porta
- **Erro de CORS**: Reinicie o backend

---

## 📚 Documentação Completa

- `README.md` - Visão geral
- `API.md` - Endpoints detalhados
- `SETUP.md` - Setup completo
- `STATUS_FINAL.md` - O que foi implementado
- `FINAL_IMPLEMENTATION.md` - Detalhes técnicos

---

## 🎯 Próximos Passos

1. ✅ Testar todas as funcionalidades
2. ✅ Configurar email de produção
3. ✅ Adicionar testes automatizados
4. ✅ Deploy em produção
5. ✅ Monitoramento e logs

---

## 🆘 Problemas?

### Backend não inicia
```bash
cd backend
npm install
npm run build
npm run dev
```

### Frontend não acessa API
```bash
# Verifique se backend está rodando
curl http://localhost:4000/health
```

### Banco não conecta
```bash
# Verifique PostgreSQL
psql -U postgres -h localhost

# Teste a conexão
npm run prisma:validate
```

---

**Bom teste! 🚀**
