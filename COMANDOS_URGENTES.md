# 🔴 COMANDOS URGENTES PARA EXECUTAR NO RENDER

## ⚡ SOLUÇÃO RÁPIDA - Execute no Render Shell

### 1️⃣ Acesse o Shell do Render
```
Dashboard → Seu serviço "inovasaude-blazor" → Shell (botão superior)
```

### 2️⃣ Execute TODOS estes comandos em sequência:

```bash
# Conectar ao banco
psql $DATABASE_URL

# Criar tabela funcionarios
CREATE TABLE IF NOT EXISTS funcionarios (
    "Id" text NOT NULL,
    "Nome" character varying(255) NOT NULL,
    "Salario" numeric(18,2) NOT NULL,
    "UbsId" text NOT NULL,
    "Cargo" character varying(50),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_funcionarios" PRIMARY KEY ("Id")
);

# Adicionar Foreign Key
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'FK_funcionarios_ubs_UbsId'
    ) THEN
        ALTER TABLE funcionarios 
        ADD CONSTRAINT "FK_funcionarios_ubs_UbsId" 
        FOREIGN KEY ("UbsId") REFERENCES ubs("Id") ON DELETE CASCADE;
    END IF;
END $$;

# Criar índice
CREATE INDEX IF NOT EXISTS "IX_funcionarios_UbsId" ON funcionarios ("UbsId");

# Verificar se funcionou
\dt funcionarios

# Sair do psql
\q
```

### 3️⃣ Reiniciar o Serviço
```
Dashboard → Seu serviço → Manual Deploy → Deploy latest commit
```

---

## 🔧 VERIFICAR LOGS DE ERRO DETALHADOS

### No Render, veja os logs:
```
Dashboard → Seu serviço → Logs
```

Procure por linhas com:
- `[ERROR]` ou `[FAIL]`
- `Microsoft.EntityFrameworkCore`
- `SaveChangesAsync`

**Cole os logs aqui para diagnóstico preciso!**

---

## 🩺 DIAGNÓSTICO COMPLETO

Se ainda houver erro, execute no Shell do Render:

```bash
# Conectar ao banco
psql $DATABASE_URL

# Verificar todas as tabelas
\dt

# Verificar estrutura da tabela funcionarios
\d funcionarios

# Verificar se UBS existe
SELECT COUNT(*) FROM ubs;

# Verificar se usuários existem
SELECT "Email", "Perfil", "Status" FROM usuarios;

# Verificar se categorias existem
SELECT "Nome" FROM categorias;

# Testar inserção manual de funcionário
INSERT INTO funcionarios ("Id", "Nome", "Salario", "UbsId", "Cargo", "CreatedAt", "UpdatedAt")
VALUES (
    gen_random_uuid()::text,
    'Teste Manual',
    2500.00,
    (SELECT "Id" FROM ubs LIMIT 1),
    'Teste',
    NOW(),
    NOW()
);

# Se funcionou, deletar o teste
DELETE FROM funcionarios WHERE "Nome" = 'Teste Manual';

# Sair
\q
```

---

## 🔌 CORRIGIR INTEGRAÇÕES (HORUS, e-SUS, NEMESIS)

### Execute no psql do Render:

```sql
-- Verificar se tabela apis_externas existe
SELECT table_name FROM information_schema.tables 
WHERE table_schema = 'public' 
AND table_name = 'apis_externas';

-- Se não existir, ela será criada pelas migrations
-- Mas você pode criar uma integração de teste assim:

INSERT INTO apis_externas (
    "Id", "Nome", "BaseUrl", "TipoAutenticacao", 
    "Token", "TimeoutSegundos", "MaxRetries", "Status",
    "TotalSincronizacoes", "TotalErros", 
    "CreatedAt", "UpdatedAt"
) VALUES (
    gen_random_uuid()::text,
    'HORUS',
    'https://horus.datasus.gov.br/api',
    'Bearer',
    'seu-token-aqui',
    30,
    3,
    'ATIVA',
    0,
    0,
    NOW(),
    NOW()
) ON CONFLICT DO NOTHING;
```

---

## 🆘 SE NADA FUNCIONAR

### Opção Nuclear - Resetar Banco:

```bash
# NO RENDER DASHBOARD (não no shell)
# Vá em: PostgreSQL Database → Settings → Delete Database
# Depois: Recriar banco e fazer novo deploy

# OU via shell:
psql $DATABASE_URL -c "DROP SCHEMA public CASCADE; CREATE SCHEMA public;"
```

Depois faça novo deploy:
```bash
git commit --allow-empty -m "trigger redeploy"
git push origin main
```

---

## 📞 PRECISA DE AJUDA?

Cole aqui:
1. ✅ Logs do Render (últimas 50 linhas)
2. ✅ Resultado do comando `\dt` no psql
3. ✅ Erro completo que aparece no navegador (F12 → Console)
